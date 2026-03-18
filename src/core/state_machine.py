from enum import Enum
from typing import Optional
import structlog

logger = structlog.get_logger()


class OrderState(str, Enum):
    ORDERED = "ORDERED"
    PICKING = "PICKING"
    PREPPING = "PREPPING"
    BAKING = "BAKING"
    DELIVERING = "DELIVERING"
    COMPLETED = "COMPLETED"
    ERROR = "ERROR"


VALID_TRANSITIONS = {
    OrderState.ORDERED: [OrderState.PICKING, OrderState.ERROR],
    OrderState.PICKING: [OrderState.PREPPING, OrderState.ERROR],
    OrderState.PREPPING: [OrderState.BAKING, OrderState.ERROR],
    OrderState.BAKING: [OrderState.DELIVERING, OrderState.ERROR],
    OrderState.DELIVERING: [OrderState.COMPLETED, OrderState.ERROR],
    OrderState.COMPLETED: [],
    OrderState.ERROR: [],
}


class InvalidTransitionError(Exception):
    def __init__(self, from_state: OrderState, to_state: OrderState):
        self.from_state = from_state
        self.to_state = to_state
        super().__init__(
            f"Cannot transition from {from_state.value} to {to_state.value}"
        )


class OrderStateMachine:
    def __init__(self, current_state: OrderState = OrderState.ORDERED):
        self.current_state = current_state

    def can_transition_to(self, target_state: OrderState) -> bool:
        return target_state in VALID_TRANSITIONS.get(self.current_state, [])

    def transition_to(self, target_state: OrderState, order_id: str, notes: Optional[str] = None) -> OrderState:
        if not self.can_transition_to(target_state):
            raise InvalidTransitionError(self.current_state, target_state)

        previous_state = self.current_state
        self.current_state = target_state

        logger.info(
            "order_state_transition",
            order_id=order_id,
            from_state=previous_state.value,
            to_state=target_state.value,
            notes=notes,
        )

        return self.current_state

    def is_terminal(self) -> bool:
        return self.current_state in (OrderState.COMPLETED, OrderState.ERROR)

    def is_active(self) -> bool:
        return not self.is_terminal()
