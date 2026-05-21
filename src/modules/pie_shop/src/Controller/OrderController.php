<?php

declare(strict_types=1);

namespace Drupal\pie_shop\Controller;

use Drupal\Core\Controller\ControllerBase;
use Drupal\pie_shop\Service\OrderService;
use Symfony\Component\DependencyInjection\ContainerInterface;
use Symfony\Component\HttpFoundation\JsonResponse;
use Symfony\Component\HttpFoundation\Request;

/**
 * Handles REST API endpoints for pie orders.
 */
class OrderController extends ControllerBase {

  public function __construct(
    private readonly OrderService $orderService,
  ) {}

  /**
   * {@inheritdoc}
   */
  public static function create(ContainerInterface $container): static {
    return new static(
      $container->get('pie_shop.order_service'),
    );
  }

  /**
   * POST /api/orders - Creates a new order.
   */
  public function create(Request $request): JsonResponse {
    $data = json_decode($request->getContent(), TRUE);

    if (json_last_error() !== JSON_ERROR_NONE) {
      return new JsonResponse(['error' => 'INVALID_JSON', 'message' => 'Request body must be valid JSON'], 400);
    }

    try {
      $order = $this->orderService->createOrder($data);
      return new JsonResponse($order, 201);
    }
    catch (\InvalidArgumentException $e) {
      return new JsonResponse([
        'error' => $e->getMessage(),
        'message' => 'Pie type is not available',
        'availableTypes' => OrderService::VALID_PIE_TYPES,
      ], 400);
    }
    catch (\Exception $e) {
      $this->getLogger('pie_shop')->error('Order creation failed: @msg', ['@msg' => $e->getMessage()]);
      return new JsonResponse(['error' => 'INTERNAL_ERROR', 'message' => 'An unexpected error occurred'], 500);
    }
  }

  /**
   * GET /api/orders/{orderId} - Retrieves a single order.
   */
  public function get(string $orderId): JsonResponse {
    try {
      $order = $this->orderService->getOrder($orderId);
      return new JsonResponse($order);
    }
    catch (\RuntimeException $e) {
      if ($e->getMessage() === 'ORDER_NOT_FOUND') {
        return new JsonResponse([
          'error' => 'ORDER_NOT_FOUND',
          'message' => "Order '{$orderId}' does not exist",
        ], 404);
      }
      return new JsonResponse(['error' => 'INTERNAL_ERROR', 'message' => 'An unexpected error occurred'], 500);
    }
  }

  /**
   * GET /api/orders - Lists all orders.
   */
  public function list(Request $request): JsonResponse {
    $filters = [
      'status' => $request->query->get('status'),
      'pie_type' => $request->query->get('pie_type'),
    ];

    $orders = $this->orderService->listOrders(array_filter($filters));

    return new JsonResponse(['orders' => $orders, 'total' => count($orders)]);
  }

  /**
   * PATCH /api/orders/{orderId} - Updates an order (admin).
   */
  public function update(string $orderId, Request $request): JsonResponse {
    $data = json_decode($request->getContent(), TRUE);

    // TODO: Add authentication middleware

    try {
      $order = $this->orderService->getOrder($orderId);
      return new JsonResponse($order);
    }
    catch (\RuntimeException $e) {
      return new JsonResponse(['error' => 'ORDER_NOT_FOUND', 'message' => "Order '{$orderId}' does not exist"], 404);
    }
  }

}
