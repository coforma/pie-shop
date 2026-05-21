<?php

declare(strict_types=1);

namespace Drupal\pie_shop\Service;

/**
 * Manages order state transitions for the pie shop workflow.
 *
 * Valid states: ORDERED -> PICKING -> PREPPING -> BAKING -> DELIVERING -> COMPLETED
 * Any state can transition to ERROR.
 */
class OrderStateMachine {

  const STATE_ORDERED = 'ORDERED';
  const STATE_PICKING = 'PICKING';
  const STATE_PREPPING = 'PREPPING';
  const STATE_BAKING = 'BAKING';
  const STATE_DELIVERING = 'DELIVERING';
  const STATE_COMPLETED = 'COMPLETED';
  const STATE_ERROR = 'ERROR';

  /**
   * Allowed transitions map.
   *
   * @var array<string, string[]>
   */
  private array $transitions = [
    self::STATE_ORDERED => [self::STATE_PICKING, self::STATE_ERROR],
    self::STATE_PICKING => [self::STATE_PREPPING, self::STATE_ERROR],
    self::STATE_PREPPING => [self::STATE_BAKING, self::STATE_ERROR],
    self::STATE_BAKING => [self::STATE_DELIVERING, self::STATE_ERROR],
    self::STATE_DELIVERING => [self::STATE_COMPLETED, self::STATE_ERROR],
    self::STATE_COMPLETED => [],
    self::STATE_ERROR => [],
  ];

  public function __construct(
    private readonly \Drupal\Core\Database\Connection $database,
    private readonly \Psr\Log\LoggerInterface $logger,
  ) {}

  /**
   * Determines whether a transition from one state to another is valid.
   */
  public function canTransition(string $fromState, string $toState): bool {
    return in_array($toState, $this->transitions[$fromState] ?? [], TRUE);
  }

  /**
   * Transitions an order to a new state and records the history entry.
   *
   * @throws \RuntimeException
   *   If the transition is not allowed.
   */
  public function transition(string $orderId, string $fromState, string $toState, string $notes = '', string $errorMessage = ''): void {
    if (!$this->canTransition($fromState, $toState)) {
      throw new \RuntimeException("Invalid transition from {$fromState} to {$toState} for order {$orderId}");
    }

    $now = \Drupal::time()->getRequestTime();

    $this->database->update('pie_shop_orders')
      ->fields([
        'current_state' => $toState,
        'updated_at' => $now,
      ])
      ->condition('id', $orderId)
      ->execute();

    $this->database->insert('pie_shop_state_history')
      ->fields([
        'order_id' => $orderId,
        'from_state' => $fromState,
        'to_state' => $toState,
        'timestamp' => $now,
        'notes' => $notes,
        'error_message' => $errorMessage,
      ])
      ->execute();

    $this->logger->info('Order @id transitioned from @from to @to', [
      '@id' => $orderId,
      '@from' => $fromState,
      '@to' => $toState,
    ]);
  }

  /**
   * Returns all valid states.
   *
   * @return string[]
   */
  public function getStates(): array {
    return array_keys($this->transitions);
  }

}
