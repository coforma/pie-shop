<?php

declare(strict_types=1);

namespace Drupal\Tests\pie_shop\Unit;

use Drupal\pie_shop\Service\OrderStateMachine;
use Drupal\Tests\UnitTestCase;
use Psr\Log\LoggerInterface;

/**
 * Unit tests for the order state machine.
 *
 * @group pie_shop
 */
class OrderStateMachineTest extends UnitTestCase {

  private OrderStateMachine $stateMachine;

  protected function setUp(): void {
    parent::setUp();

    $database = $this->createMock(\Drupal\Core\Database\Connection::class);
    $logger = $this->createMock(LoggerInterface::class);

    $this->stateMachine = new OrderStateMachine($database, $logger);
  }

  /**
   * Tests that valid forward transitions are allowed.
   *
   * @dataProvider validTransitionProvider
   */
  public function testValidTransitions(string $from, string $to): void {
    $this->assertTrue($this->stateMachine->canTransition($from, $to));
  }

  /**
   * Data provider for valid transitions.
   */
  public static function validTransitionProvider(): array {
    return [
      'ordered to picking' => [OrderStateMachine::STATE_ORDERED, OrderStateMachine::STATE_PICKING],
      'picking to prepping' => [OrderStateMachine::STATE_PICKING, OrderStateMachine::STATE_PREPPING],
      'prepping to baking' => [OrderStateMachine::STATE_PREPPING, OrderStateMachine::STATE_BAKING],
      'baking to delivering' => [OrderStateMachine::STATE_BAKING, OrderStateMachine::STATE_DELIVERING],
      'delivering to completed' => [OrderStateMachine::STATE_DELIVERING, OrderStateMachine::STATE_COMPLETED],
      'ordered to error' => [OrderStateMachine::STATE_ORDERED, OrderStateMachine::STATE_ERROR],
      'picking to error' => [OrderStateMachine::STATE_PICKING, OrderStateMachine::STATE_ERROR],
      'baking to error' => [OrderStateMachine::STATE_BAKING, OrderStateMachine::STATE_ERROR],
    ];
  }

  /**
   * Tests that invalid transitions are rejected.
   *
   * @dataProvider invalidTransitionProvider
   */
  public function testInvalidTransitions(string $from, string $to): void {
    $this->assertFalse($this->stateMachine->canTransition($from, $to));
  }

  /**
   * Data provider for invalid transitions.
   */
  public static function invalidTransitionProvider(): array {
    return [
      'ordered to baking' => [OrderStateMachine::STATE_ORDERED, OrderStateMachine::STATE_BAKING],
      'ordered to completed' => [OrderStateMachine::STATE_ORDERED, OrderStateMachine::STATE_COMPLETED],
      'completed to ordered' => [OrderStateMachine::STATE_COMPLETED, OrderStateMachine::STATE_ORDERED],
      'error to picking' => [OrderStateMachine::STATE_ERROR, OrderStateMachine::STATE_PICKING],
      'delivering to picking' => [OrderStateMachine::STATE_DELIVERING, OrderStateMachine::STATE_PICKING],
    ];
  }

  /**
   * Tests that completed state has no outgoing transitions.
   */
  public function testCompletedStateIsTerminal(): void {
    $this->assertFalse($this->stateMachine->canTransition(OrderStateMachine::STATE_COMPLETED, OrderStateMachine::STATE_ERROR));
    $this->assertFalse($this->stateMachine->canTransition(OrderStateMachine::STATE_COMPLETED, OrderStateMachine::STATE_ORDERED));
  }

  /**
   * Tests that transition() throws on invalid state change.
   */
  public function testTransitionThrowsOnInvalidChange(): void {
    $this->expectException(\RuntimeException::class);

    $database = $this->createMock(\Drupal\Core\Database\Connection::class);
    $logger = $this->createMock(LoggerInterface::class);
    $sm = new OrderStateMachine($database, $logger);

    $sm->transition('order-123', OrderStateMachine::STATE_ORDERED, OrderStateMachine::STATE_COMPLETED);
  }

  /**
   * Tests that getStates() returns all expected states.
   */
  public function testGetStatesReturnsAllStates(): void {
    $states = $this->stateMachine->getStates();

    $this->assertContains(OrderStateMachine::STATE_ORDERED, $states);
    $this->assertContains(OrderStateMachine::STATE_COMPLETED, $states);
    $this->assertContains(OrderStateMachine::STATE_ERROR, $states);
    $this->assertCount(7, $states);
  }

}
