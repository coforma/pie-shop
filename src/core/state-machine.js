/**
 * State Machine for Order Orchestration
 * 
 * Manages the lifecycle of pie orders through various states.
 * This is a clean implementation of the state pattern.
 */

const OrderStates = {
  ORDERED: 'ORDERED',
  PICKING: 'PICKING',
  PREPPING: 'PREPPING',
  BAKING: 'BAKING',
  DELIVERING: 'DELIVERING',
  COMPLETED: 'COMPLETED',
  ERROR: 'ERROR'
};

const StateTransitions = {
  [OrderStates.ORDERED]: [OrderStates.PICKING, OrderStates.ERROR],
  [OrderStates.PICKING]: [OrderStates.PREPPING, OrderStates.ERROR],
  [OrderStates.PREPPING]: [OrderStates.BAKING, OrderStates.ERROR],
  [OrderStates.BAKING]: [OrderStates.DELIVERING, OrderStates.ERROR],
  [OrderStates.DELIVERING]: [OrderStates.COMPLETED, OrderStates.ERROR],
  [OrderStates.ERROR]: [],
  [OrderStates.COMPLETED]: []
};

class StateMachine {
  /**
   * Validates if a state transition is allowed
   * @param {string} fromState - Current state
   * @param {string} toState - Target state
   * @returns {boolean} - Whether transition is valid
   */
  canTransition(fromState, toState) {
    if (!StateTransitions[fromState]) {
      return false;
    }
    return StateTransitions[fromState].includes(toState);
  }

  /**
   * Gets the next logical state in the order flow
   * @param {string} currentState - Current order state
   * @returns {string|null} - Next state or null if at end
   */
  getNextState(currentState) {
    const validTransitions = StateTransitions[currentState];
    if (!validTransitions || validTransitions.length === 0) {
      return null;
    }
    return validTransitions[0];
  }

  /**
   * Validates that a state is a known state
   * @param {string} state - State to validate
   * @returns {boolean} - Whether state is valid
   */
  isValidState(state) {
    return Object.values(OrderStates).includes(state);
  }
}

module.exports = {
  StateMachine,
  OrderStates,
  StateTransitions
};
