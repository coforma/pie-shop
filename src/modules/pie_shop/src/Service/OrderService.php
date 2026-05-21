<?php

declare(strict_types=1);

namespace Drupal\pie_shop\Service;

use Drupal\Core\Database\Connection;
use Psr\Log\LoggerInterface;
use Ramsey\Uuid\Uuid;

/**
 * Handles order creation, retrieval, and workflow orchestration.
 */
class OrderService {

  const VALID_PIE_TYPES = ['apple', 'cherry', 'pumpkin', 'pecan', 'blueberry'];

  // TODO: Move to configuration
  const DEFAULT_DELIVERY_HOURS = 3;

  public function __construct(
    private readonly Connection $database,
    private readonly OrderStateMachine $stateMachine,
    private readonly FruitPickerClient $fruitPickerClient,
    private readonly BakerClient $bakerClient,
    private readonly DeliveryClient $deliveryClient,
    private readonly LoggerInterface $logger,
  ) {}

  /**
   * Creates a new pie order and initiates the workflow.
   *
   * @param array $data
   *   Order data from the API request.
   *
   * @return array
   *   Created order summary.
   *
   * @throws \InvalidArgumentException
   *   If required fields are missing or invalid.
   */
  public function createOrder(array $data): array {
    if (empty($data['pieType']) || !in_array($data['pieType'], self::VALID_PIE_TYPES, TRUE)) {
      throw new \InvalidArgumentException('INVALID_PIE_TYPE');
    }

    $customer = $data['customer'] ?? [];
    $address = $data['deliveryAddress'] ?? [];

    $orderId = Uuid::uuid4()->toString();
    $now = \Drupal::time()->getRequestTime();
    $estimatedDelivery = $now + (self::DEFAULT_DELIVERY_HOURS * 3600);

    $this->database->insert('pie_shop_orders')
      ->fields([
        'id' => $orderId,
        'pie_type' => $data['pieType'],
        'customer_name' => $customer['name'] ?? '',
        'customer_email' => $customer['email'] ?? '',
        'customer_phone' => $customer['phone'] ?? '',
        'delivery_street' => $address['street'] ?? '',
        'delivery_city' => $address['city'] ?? '',
        'delivery_state' => $address['state'] ?? '',
        'delivery_zip' => $address['zip'] ?? '',
        'current_state' => OrderStateMachine::STATE_ORDERED,
        'estimated_delivery' => $estimatedDelivery,
        'created_at' => $now,
        'updated_at' => $now,
      ])
      ->execute();

    $this->database->insert('pie_shop_state_history')
      ->fields([
        'order_id' => $orderId,
        'from_state' => '',
        'to_state' => OrderStateMachine::STATE_ORDERED,
        'timestamp' => $now,
        'notes' => 'Order created',
        'error_message' => '',
      ])
      ->execute();

    // Kick off the workflow asynchronously via queue
    $queue = \Drupal::queue('pie_shop_order_processor');
    $queue->createItem(['order_id' => $orderId, 'action' => 'start_picking']);

    $this->logger->info('Order @id created for @type pie', [
      '@id' => $orderId,
      '@type' => $data['pieType'],
    ]);

    return [
      'orderId' => $orderId,
      'status' => OrderStateMachine::STATE_ORDERED,
      'estimatedDelivery' => date('c', $estimatedDelivery),
      'createdAt' => date('c', $now),
    ];
  }

  /**
   * Retrieves a single order with its state history.
   *
   * @param string $orderId
   *   The order UUID.
   *
   * @return array
   *   Order data including history.
   *
   * @throws \RuntimeException
   *   If the order does not exist.
   */
  public function getOrder(string $orderId): array {
    $order = $this->database->select('pie_shop_orders', 'o')
      ->fields('o')
      ->condition('o.id', $orderId)
      ->execute()
      ->fetchAssoc();

    if (!$order) {
      throw new \RuntimeException('ORDER_NOT_FOUND');
    }

    $history = $this->database->select('pie_shop_state_history', 'h')
      ->fields('h', ['from_state', 'to_state', 'timestamp', 'notes', 'error_message'])
      ->condition('h.order_id', $orderId)
      ->orderBy('h.timestamp', 'ASC')
      ->execute()
      ->fetchAll(\PDO::FETCH_ASSOC);

    return $this->formatOrder($order, $history);
  }

  /**
   * Returns a list of all orders with optional filters.
   *
   * @param array $filters
   *   Optional filters: status, pie_type, date_from, date_to.
   *
   * @return array
   *   List of orders.
   */
  public function listOrders(array $filters = []): array {
    $query = $this->database->select('pie_shop_orders', 'o')
      ->fields('o')
      ->orderBy('o.created_at', 'DESC');

    if (!empty($filters['status'])) {
      $query->condition('o.current_state', $filters['status']);
    }

    if (!empty($filters['pie_type'])) {
      $query->condition('o.pie_type', $filters['pie_type']);
    }

    // TODO: Add pagination

    $orders = $query->execute()->fetchAll(\PDO::FETCH_ASSOC);

    return array_map(fn($order) => $this->formatOrder($order, []), $orders);
  }

  /**
   * Advances an order through the picking stage.
   *
   * Calls the fruit picker service and transitions the order to PICKING.
   * On failure, transitions to ERROR.
   */
  public function startPicking(string $orderId): void {
    $order = $this->database->select('pie_shop_orders', 'o')
      ->fields('o')
      ->condition('o.id', $orderId)
      ->execute()
      ->fetchAssoc();

    if (!$order) {
      return;
    }

    try {
      $this->stateMachine->transition($orderId, $order['current_state'], OrderStateMachine::STATE_PICKING);

      $result = $this->fruitPickerClient->pickFruit($order['pie_type'], 6);

      $this->database->update('pie_shop_orders')
        ->fields(['picker_job_id' => $result['jobId']])
        ->condition('id', $orderId)
        ->execute();

      $queue = \Drupal::queue('pie_shop_order_processor');
      $queue->createItem([
        'order_id' => $orderId,
        'action' => 'check_picking',
        'job_id' => $result['jobId'],
        'attempts' => 0,
      ]);
    }
    catch (\Exception $e) {
      $this->logger->error('Picking failed for order @id: @msg', [
        '@id' => $orderId,
        '@msg' => $e->getMessage(),
      ]);
      $this->stateMachine->transition($orderId, OrderStateMachine::STATE_PICKING, OrderStateMachine::STATE_ERROR, '', $e->getMessage());
    }
  }

  /**
   * Polls the fruit picker service and advances to PREPPING when complete.
   *
   * Basic retry logic - checks job status and re-queues if still in progress.
   */
  public function checkPickingStatus(string $orderId, string $jobId, int $attempts): void {
    $maxAttempts = 10;

    if ($attempts >= $maxAttempts) {
      $order = $this->getOrderRow($orderId);
      $this->stateMachine->transition($orderId, $order['current_state'], OrderStateMachine::STATE_ERROR, '', 'Picking timed out');
      return;
    }

    try {
      $status = $this->fruitPickerClient->getJobStatus($jobId);

      if ($status['status'] === 'COMPLETED') {
        $order = $this->getOrderRow($orderId);
        $this->stateMachine->transition($orderId, $order['current_state'], OrderStateMachine::STATE_PREPPING, 'Ingredients received');
        $this->startBaking($orderId);
      }
      elseif ($status['status'] === 'FAILED') {
        $order = $this->getOrderRow($orderId);
        $this->stateMachine->transition($orderId, $order['current_state'], OrderStateMachine::STATE_ERROR, '', 'Picker service reported failure');
      }
      else {
        $queue = \Drupal::queue('pie_shop_order_processor');
        $queue->createItem([
          'order_id' => $orderId,
          'action' => 'check_picking',
          'job_id' => $jobId,
          'attempts' => $attempts + 1,
        ]);
      }
    }
    catch (\Exception $e) {
      $this->logger->warning('Picking status check failed for @id (attempt @n): @msg', [
        '@id' => $orderId,
        '@n' => $attempts,
        '@msg' => $e->getMessage(),
      ]);
      $queue = \Drupal::queue('pie_shop_order_processor');
      $queue->createItem([
        'order_id' => $orderId,
        'action' => 'check_picking',
        'job_id' => $jobId,
        'attempts' => $attempts + 1,
      ]);
    }
  }

  /**
   * Schedules baking with the baker service and transitions to BAKING.
   */
  public function startBaking(string $orderId): void {
    $order = $this->getOrderRow($orderId);

    $recipes = [
      'apple' => ['temp' => 375, 'duration' => 45],
      'cherry' => ['temp' => 400, 'duration' => 40],
      'pumpkin' => ['temp' => 350, 'duration' => 55],
      'pecan' => ['temp' => 350, 'duration' => 50],
      'blueberry' => ['temp' => 375, 'duration' => 45],
    ];

    $recipe = $recipes[$order['pie_type']] ?? ['temp' => 375, 'duration' => 45];

    try {
      $this->stateMachine->transition($orderId, $order['current_state'], OrderStateMachine::STATE_BAKING);

      $result = $this->bakerClient->bake($order['pie_type'], $recipe['temp'], $recipe['duration']);

      $this->database->update('pie_shop_orders')
        ->fields(['baker_job_id' => $result['jobId']])
        ->condition('id', $orderId)
        ->execute();

      $queue = \Drupal::queue('pie_shop_order_processor');
      $queue->createItem([
        'order_id' => $orderId,
        'action' => 'check_baking',
        'job_id' => $result['jobId'],
        'attempts' => 0,
      ]);
    }
    catch (\Exception $e) {
      $this->logger->error('Baking failed for order @id: @msg', [
        '@id' => $orderId,
        '@msg' => $e->getMessage(),
      ]);
      $this->stateMachine->transition($orderId, OrderStateMachine::STATE_BAKING, OrderStateMachine::STATE_ERROR, '', $e->getMessage());
    }
  }

  /**
   * Polls baker service and advances to DELIVERING when baking is complete.
   */
  public function checkBakingStatus(string $orderId, string $jobId, int $attempts): void {
    // No retry for baking - burnt pie is a burnt pie
    if ($attempts >= 5) {
      $order = $this->getOrderRow($orderId);
      $this->stateMachine->transition($orderId, $order['current_state'], OrderStateMachine::STATE_ERROR, '', 'Baking timed out');
      return;
    }

    try {
      $status = $this->bakerClient->getJobStatus($jobId);

      if ($status['status'] === 'COMPLETED') {
        $order = $this->getOrderRow($orderId);
        $this->stateMachine->transition($orderId, $order['current_state'], OrderStateMachine::STATE_DELIVERING, 'Pie is ready');
        $this->startDelivery($orderId);
      }
      elseif ($status['status'] === 'FAILED') {
        $order = $this->getOrderRow($orderId);
        $this->stateMachine->transition($orderId, $order['current_state'], OrderStateMachine::STATE_ERROR, '', 'Baker service reported failure');
      }
      else {
        $queue = \Drupal::queue('pie_shop_order_processor');
        $queue->createItem([
          'order_id' => $orderId,
          'action' => 'check_baking',
          'job_id' => $jobId,
          'attempts' => $attempts + 1,
        ]);
      }
    }
    catch (\Exception $e) {
      $queue = \Drupal::queue('pie_shop_order_processor');
      $queue->createItem([
        'order_id' => $orderId,
        'action' => 'check_baking',
        'job_id' => $jobId,
        'attempts' => $attempts + 1,
      ]);
    }
  }

  /**
   * Arranges drone delivery and transitions to DELIVERING.
   */
  public function startDelivery(string $orderId): void {
    $order = $this->getOrderRow($orderId);

    $destination = [
      'street' => $order['delivery_street'],
      'city' => $order['delivery_city'],
      'state' => $order['delivery_state'],
      'zip' => $order['delivery_zip'],
    ];

    $window = date('c', time() + 3600) . '/' . date('c', time() + 7200);

    try {
      $result = $this->deliveryClient->scheduleDelivery(
        ['type' => $order['pie_type'], 'size' => 'standard'],
        $destination,
        $window
      );

      $this->database->update('pie_shop_orders')
        ->fields(['delivery_id' => $result['deliveryId']])
        ->condition('id', $orderId)
        ->execute();

      $queue = \Drupal::queue('pie_shop_order_processor');
      $queue->createItem([
        'order_id' => $orderId,
        'action' => 'check_delivery',
        'delivery_id' => $result['deliveryId'],
        'attempts' => 0,
      ]);
    }
    catch (\Exception $e) {
      $this->logger->error('Delivery scheduling failed for order @id: @msg', [
        '@id' => $orderId,
        '@msg' => $e->getMessage(),
      ]);
      $this->stateMachine->transition($orderId, $order['current_state'], OrderStateMachine::STATE_ERROR, '', $e->getMessage());
    }
  }

  /**
   * Polls delivery service and marks order COMPLETED on delivery.
   */
  public function checkDeliveryStatus(string $orderId, string $deliveryId, int $attempts): void {
    if ($attempts >= 20) {
      $order = $this->getOrderRow($orderId);
      $this->stateMachine->transition($orderId, $order['current_state'], OrderStateMachine::STATE_ERROR, '', 'Delivery timed out');
      return;
    }

    try {
      $status = $this->deliveryClient->getDeliveryStatus($deliveryId);

      if ($status['status'] === 'DELIVERED') {
        $order = $this->getOrderRow($orderId);
        $this->stateMachine->transition($orderId, $order['current_state'], OrderStateMachine::STATE_COMPLETED, 'Delivered successfully');
      }
      elseif ($status['status'] === 'FAILED') {
        $order = $this->getOrderRow($orderId);
        $this->stateMachine->transition($orderId, $order['current_state'], OrderStateMachine::STATE_ERROR, '', 'Delivery failed: ' . ($status['reason'] ?? 'unknown'));
      }
      else {
        $queue = \Drupal::queue('pie_shop_order_processor');
        $queue->createItem([
          'order_id' => $orderId,
          'action' => 'check_delivery',
          'delivery_id' => $deliveryId,
          'attempts' => $attempts + 1,
        ]);
      }
    }
    catch (\Exception $e) {
      $queue = \Drupal::queue('pie_shop_order_processor');
      $queue->createItem([
        'order_id' => $orderId,
        'action' => 'check_delivery',
        'delivery_id' => $deliveryId,
        'attempts' => $attempts + 1,
      ]);
    }
  }

  /**
   * Fetches a raw order row from the database.
   */
  private function getOrderRow(string $orderId): array {
    $order = $this->database->select('pie_shop_orders', 'o')
      ->fields('o')
      ->condition('o.id', $orderId)
      ->execute()
      ->fetchAssoc();

    if (!$order) {
      throw new \RuntimeException("Order {$orderId} not found");
    }

    return $order;
  }

  /**
   * Formats a raw database row into the API response shape.
   */
  private function formatOrder(array $order, array $history): array {
    return [
      'orderId' => $order['id'],
      'pieType' => $order['pie_type'],
      'customer' => [
        'name' => $order['customer_name'],
        'email' => $order['customer_email'],
        'phone' => $order['customer_phone'],
      ],
      'deliveryAddress' => [
        'street' => $order['delivery_street'],
        'city' => $order['delivery_city'],
        'state' => $order['delivery_state'],
        'zip' => $order['delivery_zip'],
      ],
      'status' => $order['current_state'],
      'createdAt' => date('c', (int) $order['created_at']),
      'updatedAt' => date('c', (int) $order['updated_at']),
      'estimatedDelivery' => $order['estimated_delivery'] ? date('c', (int) $order['estimated_delivery']) : NULL,
      'history' => array_map(fn($h) => [
        'state' => $h['to_state'],
        'timestamp' => date('c', (int) $h['timestamp']),
        'notes' => $h['notes'],
      ], $history),
    ];
  }

}
