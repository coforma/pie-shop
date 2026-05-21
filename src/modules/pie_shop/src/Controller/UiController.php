<?php

declare(strict_types=1);

namespace Drupal\pie_shop\Controller;

use Drupal\Core\Controller\ControllerBase;
use Drupal\pie_shop\Service\OrderService;
use Symfony\Component\DependencyInjection\ContainerInterface;
use Symfony\Component\HttpFoundation\Response;

/**
 * Serves the customer order form and admin dashboard UI.
 */
class UiController extends ControllerBase {

  public function __construct(
    private readonly OrderService $orderService,
  ) {}

  /**
   * {@inheritdoc}
   */
  public static function create(ContainerInterface $container): static {
    return new static($container->get('pie_shop.order_service'));
  }

  /**
   * Renders the customer order form.
   */
  public function orderForm(): array {
    return [
      '#theme' => 'pie_shop_order_form',
      '#attached' => [
        'library' => ['pie_shop/order_form'],
      ],
    ];
  }

  /**
   * Renders the admin dashboard.
   */
  public function adminDashboard(): array {
    $orders = $this->orderService->listOrders();

    return [
      '#theme' => 'pie_shop_admin_dashboard',
      '#orders' => $orders,
      '#attached' => [
        'library' => ['pie_shop/admin_dashboard'],
      ],
    ];
  }

}
