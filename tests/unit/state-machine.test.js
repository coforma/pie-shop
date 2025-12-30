const { StateMachine, OrderStates, StateTransitions } = require('../../src/core/state-machine');

describe('StateMachine', () => {
  let stateMachine;

  beforeEach(() => {
    stateMachine = new StateMachine();
  });

  describe('canTransition', () => {
    test('should allow transition from ORDERED to PICKING', () => {
      expect(stateMachine.canTransition(OrderStates.ORDERED, OrderStates.PICKING)).toBe(true);
    });

    test('should allow transition from PICKING to PREPPING', () => {
      expect(stateMachine.canTransition(OrderStates.PICKING, OrderStates.PREPPING)).toBe(true);
    });

    test('should allow transition from PREPPING to BAKING', () => {
      expect(stateMachine.canTransition(OrderStates.PREPPING, OrderStates.BAKING)).toBe(true);
    });

    test('should allow transition from BAKING to DELIVERING', () => {
      expect(stateMachine.canTransition(OrderStates.BAKING, OrderStates.DELIVERING)).toBe(true);
    });

    test('should allow transition from DELIVERING to COMPLETED', () => {
      expect(stateMachine.canTransition(OrderStates.DELIVERING, OrderStates.COMPLETED)).toBe(true);
    });

    test('should allow transition to ERROR from any active state', () => {
      expect(stateMachine.canTransition(OrderStates.ORDERED, OrderStates.ERROR)).toBe(true);
      expect(stateMachine.canTransition(OrderStates.PICKING, OrderStates.ERROR)).toBe(true);
      expect(stateMachine.canTransition(OrderStates.PREPPING, OrderStates.ERROR)).toBe(true);
      expect(stateMachine.canTransition(OrderStates.BAKING, OrderStates.ERROR)).toBe(true);
      expect(stateMachine.canTransition(OrderStates.DELIVERING, OrderStates.ERROR)).toBe(true);
    });

    test('should not allow transition from ORDERED to BAKING', () => {
      expect(stateMachine.canTransition(OrderStates.ORDERED, OrderStates.BAKING)).toBe(false);
    });

    test('should not allow transition from COMPLETED to any state', () => {
      expect(stateMachine.canTransition(OrderStates.COMPLETED, OrderStates.ORDERED)).toBe(false);
      expect(stateMachine.canTransition(OrderStates.COMPLETED, OrderStates.PICKING)).toBe(false);
    });

    test('should not allow transition from ERROR to any state', () => {
      expect(stateMachine.canTransition(OrderStates.ERROR, OrderStates.ORDERED)).toBe(false);
      expect(stateMachine.canTransition(OrderStates.ERROR, OrderStates.PICKING)).toBe(false);
    });

    test('should not allow backward transitions', () => {
      expect(stateMachine.canTransition(OrderStates.BAKING, OrderStates.PREPPING)).toBe(false);
      expect(stateMachine.canTransition(OrderStates.DELIVERING, OrderStates.BAKING)).toBe(false);
    });
  });

  describe('getNextState', () => {
    test('should return PICKING as next state from ORDERED', () => {
      expect(stateMachine.getNextState(OrderStates.ORDERED)).toBe(OrderStates.PICKING);
    });

    test('should return PREPPING as next state from PICKING', () => {
      expect(stateMachine.getNextState(OrderStates.PICKING)).toBe(OrderStates.PREPPING);
    });

    test('should return BAKING as next state from PREPPING', () => {
      expect(stateMachine.getNextState(OrderStates.PREPPING)).toBe(OrderStates.BAKING);
    });

    test('should return DELIVERING as next state from BAKING', () => {
      expect(stateMachine.getNextState(OrderStates.BAKING)).toBe(OrderStates.DELIVERING);
    });

    test('should return COMPLETED as next state from DELIVERING', () => {
      expect(stateMachine.getNextState(OrderStates.DELIVERING)).toBe(OrderStates.COMPLETED);
    });

    test('should return null for COMPLETED state', () => {
      expect(stateMachine.getNextState(OrderStates.COMPLETED)).toBe(null);
    });

    test('should return null for ERROR state', () => {
      expect(stateMachine.getNextState(OrderStates.ERROR)).toBe(null);
    });
  });

  describe('isValidState', () => {
    test('should return true for all valid states', () => {
      Object.values(OrderStates).forEach(state => {
        expect(stateMachine.isValidState(state)).toBe(true);
      });
    });

    test('should return false for invalid states', () => {
      expect(stateMachine.isValidState('INVALID')).toBe(false);
      expect(stateMachine.isValidState('CANCELLED')).toBe(false);
      expect(stateMachine.isValidState('')).toBe(false);
    });
  });
});
