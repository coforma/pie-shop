import pytest
from src.core.state_machine import (
    OrderStateMachine,
    OrderState,
    InvalidTransitionError,
)


class TestOrderStateMachine:
    def test_initial_state_is_ordered(self):
        sm = OrderStateMachine()
        assert sm.current_state == OrderState.ORDERED

    def test_ordered_can_transition_to_picking(self):
        sm = OrderStateMachine(OrderState.ORDERED)
        assert sm.can_transition_to(OrderState.PICKING) is True

    def test_ordered_can_transition_to_error(self):
        sm = OrderStateMachine(OrderState.ORDERED)
        assert sm.can_transition_to(OrderState.ERROR) is True

    def test_ordered_cannot_skip_to_baking(self):
        sm = OrderStateMachine(OrderState.ORDERED)
        assert sm.can_transition_to(OrderState.BAKING) is False

    def test_ordered_cannot_go_to_completed(self):
        sm = OrderStateMachine(OrderState.ORDERED)
        assert sm.can_transition_to(OrderState.COMPLETED) is False

    def test_valid_full_workflow_transitions(self):
        sm = OrderStateMachine(OrderState.ORDERED)

        sm.transition_to(OrderState.PICKING, "test-order-1")
        assert sm.current_state == OrderState.PICKING

        sm.transition_to(OrderState.PREPPING, "test-order-1")
        assert sm.current_state == OrderState.PREPPING

        sm.transition_to(OrderState.BAKING, "test-order-1")
        assert sm.current_state == OrderState.BAKING

        sm.transition_to(OrderState.DELIVERING, "test-order-1")
        assert sm.current_state == OrderState.DELIVERING

        sm.transition_to(OrderState.COMPLETED, "test-order-1")
        assert sm.current_state == OrderState.COMPLETED

    def test_invalid_transition_raises_error(self):
        sm = OrderStateMachine(OrderState.ORDERED)
        with pytest.raises(InvalidTransitionError) as exc_info:
            sm.transition_to(OrderState.COMPLETED, "test-order-1")
        assert exc_info.value.from_state == OrderState.ORDERED
        assert exc_info.value.to_state == OrderState.COMPLETED

    def test_transition_to_error_from_any_active_state(self):
        active_states = [
            OrderState.ORDERED,
            OrderState.PICKING,
            OrderState.PREPPING,
            OrderState.BAKING,
            OrderState.DELIVERING,
        ]
        for state in active_states:
            sm = OrderStateMachine(state)
            sm.transition_to(OrderState.ERROR, "test-order-1")
            assert sm.current_state == OrderState.ERROR

    def test_completed_is_terminal(self):
        sm = OrderStateMachine(OrderState.COMPLETED)
        assert sm.is_terminal() is True
        assert sm.is_active() is False

    def test_error_is_terminal(self):
        sm = OrderStateMachine(OrderState.ERROR)
        assert sm.is_terminal() is True

    def test_in_progress_states_are_active(self):
        active_states = [OrderState.PICKING, OrderState.PREPPING, OrderState.BAKING]
        for state in active_states:
            sm = OrderStateMachine(state)
            assert sm.is_active() is True

    def test_cannot_transition_from_terminal_completed(self):
        sm = OrderStateMachine(OrderState.COMPLETED)
        assert sm.can_transition_to(OrderState.PICKING) is False
        assert sm.can_transition_to(OrderState.ERROR) is False

    def test_cannot_transition_from_terminal_error(self):
        sm = OrderStateMachine(OrderState.ERROR)
        assert sm.can_transition_to(OrderState.PICKING) is False

    def test_transition_returns_new_state(self):
        sm = OrderStateMachine(OrderState.ORDERED)
        result = sm.transition_to(OrderState.PICKING, "test-order-1")
        assert result == OrderState.PICKING

    def test_transition_with_notes(self):
        sm = OrderStateMachine(OrderState.ORDERED)
        sm.transition_to(OrderState.PICKING, "test-order-1", notes="Picking started")
        assert sm.current_state == OrderState.PICKING
