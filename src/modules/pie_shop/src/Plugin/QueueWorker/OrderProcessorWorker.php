<?php

declare(strict_types=1);

namespace Drupal\pie_shop\Plugin\QueueWorker;

use Drupal\Core\Plugin\ContainerFactoryPluginInterface;
use Drupal\Core\Queue\QueueWorkerBase;
use Drupal\pie_shop\Service\OrderService;
use Symfony\Component\DependencyInjection\ContainerInterface;

/**
 * Processes pie order workflow steps.
 *
 * @QueueWorker(
 *   id = "pie_shop_order_processor",
 *   title = @Translation("Pie Shop Order Processor"),
 *   cron = {"time" = 30}
 * )
 */
class OrderProcessorWorker extends QueueWorkerBase implements ContainerFactoryPluginInterface {

  public function __construct(
    array $configuration,
    string $pluginId,
    mixed $pluginDefinition,
    private readonly OrderService $orderService,
  ) {
    parent::__construct($configuration, $pluginId, $pluginDefinition);
  }

  /**
   * {@inheritdoc}
   */
  public static function create(ContainerInterface $container, array $configuration, $pluginId, $pluginDefinition): static {
    return new static(
      $configuration,
      $pluginId,
      $pluginDefinition,
      $container->get('pie_shop.order_service'),
    );
  }

  /**
   * {@inheritdoc}
   */
  public function processItem(mixed $data): void {
    $orderId = $data['order_id'] ?? '';
    $action = $data['action'] ?? '';

    match ($action) {
      'start_picking' => $this->orderService->startPicking($orderId),
      'check_picking' => $this->orderService->checkPickingStatus($orderId, $data['job_id'], $data['attempts'] ?? 0),
      'check_baking' => $this->orderService->checkBakingStatus($orderId, $data['job_id'], $data['attempts'] ?? 0),
      'check_delivery' => $this->orderService->checkDeliveryStatus($orderId, $data['delivery_id'], $data['attempts'] ?? 0),
      default => throw new \InvalidArgumentException("Unknown action: {$action}"),
    };
  }

}
