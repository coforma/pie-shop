from datetime import datetime, timedelta
from typing import Optional, List
import structlog
from sqlalchemy.ext.asyncio import AsyncSession
from sqlalchemy import select, update, func

from src.models.order import Order, StateHistory
from src.models.recipe import PIE_CATALOG
from src.core.state_machine import OrderStateMachine, OrderState, InvalidTransitionError
from src.services.fruit_picker_client import FruitPickerClient
from src.services.baker_client import BakerClient
from src.services.delivery_client import DeliveryClient

logger = structlog.get_logger()

VALID_PIE_TYPES = list(PIE_CATALOG.keys())


class OrderNotFoundError(Exception):
    pass


class InvalidOrderError(Exception):
    pass


class OrderService:
    def __init__(self, db: AsyncSession):
        self.db = db
        self.fruit_picker = FruitPickerClient()
        self.baker = BakerClient()
        self.delivery = DeliveryClient()

    async def create_order(self, pie_type: str, customer: dict, delivery_address: dict) -> Order:
        if pie_type not in VALID_PIE_TYPES:
            raise InvalidOrderError(f"Pie type '{pie_type}' is not available")

        if not customer.get("name") or not customer.get("email"):
            raise InvalidOrderError("Customer name and email are required")

        estimated_delivery = datetime.utcnow() + timedelta(hours=3)

        order = Order(
            pie_type=pie_type,
            customer_name=customer["name"],
            customer_email=customer["email"],
            customer_phone=customer.get("phone"),
            delivery_street=delivery_address["street"],
            delivery_city=delivery_address["city"],
            delivery_state=delivery_address["state"],
            delivery_zip=delivery_address["zip"],
            current_state=OrderState.ORDERED.value,
            estimated_delivery=estimated_delivery,
        )

        self.db.add(order)
        await self.db.flush()

        history = StateHistory(
            order_id=order.id,
            from_state=None,
            to_state=OrderState.ORDERED.value,
            notes="Order created",
        )
        self.db.add(history)
        await self.db.commit()
        await self.db.refresh(order)

        logger.info("order_created", order_id=order.id, pie_type=pie_type)
        return order

    async def get_order(self, order_id: str) -> Order:
        result = await self.db.execute(select(Order).where(Order.id == order_id))
        order = result.scalar_one_or_none()
        if not order:
            raise OrderNotFoundError(f"Order '{order_id}' does not exist")
        return order

    async def list_orders(self, status: Optional[str] = None, pie_type: Optional[str] = None) -> List[Order]:
        query = select(Order)
        if status:
            query = query.where(Order.current_state == status)
        if pie_type:
            query = query.where(Order.pie_type == pie_type)
        query = query.order_by(Order.created_at.desc())
        result = await self.db.execute(query)
        return list(result.scalars().all())

    async def advance_order(self, order_id: str) -> Order:
        order = await self.get_order(order_id)
        recipe = PIE_CATALOG.get(order.pie_type, {})
        current_state = OrderState(order.current_state)
        sm = OrderStateMachine(current_state)

        if current_state == OrderState.ORDERED:
            fruit_type = order.pie_type
            quantity = len(recipe.get("ingredients", []))

            try:
                pick_response = self.fruit_picker.pick_fruit(fruit_type, quantity)
                order.picker_job_id = pick_response.get("jobId")
                sm.transition_to(OrderState.PICKING, order_id, "Started fruit picking")
                order.current_state = OrderState.PICKING.value
                history = StateHistory(
                    order_id=order.id,
                    from_state=OrderState.ORDERED.value,
                    to_state=OrderState.PICKING.value,
                    notes="Fruit picking started",
                )
                self.db.add(history)
                await self.db.commit()
                await self.db.refresh(order)
                return order
            except Exception as e:
                logger.error("picking_failed", order_id=order_id, error=str(e))
                order.current_state = OrderState.ERROR.value
                order.error_message = str(e)
                history = StateHistory(
                    order_id=order.id,
                    from_state=OrderState.ORDERED.value,
                    to_state=OrderState.ERROR.value,
                    error_message=str(e),
                )
                self.db.add(history)
                await self.db.commit()
                await self.db.refresh(order)
                return order

        elif current_state == OrderState.PICKING:
            try:
                sm.transition_to(OrderState.PREPPING, order_id, "Ingredients received, starting prep")
                order.current_state = OrderState.PREPPING.value
                history = StateHistory(
                    order_id=order.id,
                    from_state=OrderState.PICKING.value,
                    to_state=OrderState.PREPPING.value,
                    notes="Ingredients received",
                )
                self.db.add(history)
                await self.db.commit()
                await self.db.refresh(order)
                return order
            except Exception as e:
                logger.error("prepping_transition_failed", order_id=order_id, error=str(e))
                raise

        elif current_state == OrderState.PREPPING:
            baking_temp = recipe.get("bakingTemp", 350)
            baking_time = recipe.get("bakingTime", 45)

            try:
                bake_response = self.baker.start_baking(order.pie_type, baking_temp, baking_time)
                order.baker_job_id = bake_response.get("jobId")
                sm.transition_to(OrderState.BAKING, order_id, "Baking started")
                order.current_state = OrderState.BAKING.value
                history = StateHistory(
                    order_id=order.id,
                    from_state=OrderState.PREPPING.value,
                    to_state=OrderState.BAKING.value,
                    notes=f"Baking at {baking_temp}F for {baking_time} minutes",
                )
                self.db.add(history)
                await self.db.commit()
                await self.db.refresh(order)
                return order
            except Exception as e:
                logger.error("baking_failed", order_id=order_id, error=str(e))
                order.current_state = OrderState.ERROR.value
                order.error_message = str(e)
                history = StateHistory(
                    order_id=order.id,
                    from_state=OrderState.PREPPING.value,
                    to_state=OrderState.ERROR.value,
                    error_message=str(e),
                )
                self.db.add(history)
                await self.db.commit()
                await self.db.refresh(order)
                return order

        elif current_state == OrderState.BAKING:
            window_start = datetime.utcnow()
            window_end = window_start + timedelta(hours=2)

            delivery_address = {
                "street": order.delivery_street,
                "city": order.delivery_city,
                "state": order.delivery_state,
                "zip": order.delivery_zip,
            }

            try:
                delivery_response = self.delivery.schedule_delivery(
                    order_id,
                    delivery_address,
                    window_start.isoformat() + "Z",
                    window_end.isoformat() + "Z",
                )
                order.delivery_id = delivery_response.get("deliveryId")
                order.estimated_delivery = datetime.fromisoformat(
                    delivery_response.get("eta", window_end.isoformat()).replace("Z", "")
                )
                sm.transition_to(OrderState.DELIVERING, order_id, "Pie picked up for delivery")
                order.current_state = OrderState.DELIVERING.value
                history = StateHistory(
                    order_id=order.id,
                    from_state=OrderState.BAKING.value,
                    to_state=OrderState.DELIVERING.value,
                    notes=f"Delivering via drone {delivery_response.get('droneId', 'unknown')}",
                )
                self.db.add(history)
                await self.db.commit()
                await self.db.refresh(order)
                return order
            except Exception as e:
                logger.error("delivery_failed", order_id=order_id, error=str(e))
                order.current_state = OrderState.ERROR.value
                order.error_message = str(e)
                history = StateHistory(
                    order_id=order.id,
                    from_state=OrderState.BAKING.value,
                    to_state=OrderState.ERROR.value,
                    error_message=str(e),
                )
                self.db.add(history)
                await self.db.commit()
                await self.db.refresh(order)
                return order

        elif current_state == OrderState.DELIVERING:
            sm.transition_to(OrderState.COMPLETED, order_id, "Delivery confirmed")
            order.current_state = OrderState.COMPLETED.value
            history = StateHistory(
                order_id=order.id,
                from_state=OrderState.DELIVERING.value,
                to_state=OrderState.COMPLETED.value,
                notes="Order delivered successfully",
            )
            self.db.add(history)
            await self.db.commit()
            await self.db.refresh(order)
            return order

        else:
            raise InvalidOrderError(f"Order in terminal state: {current_state.value}")

    async def get_order_count_by_status(self) -> dict:
        result = await self.db.execute(
            select(Order.current_state, func.count(Order.id)).group_by(Order.current_state)
        )
        rows = result.all()
        return {row[0]: row[1] for row in rows}
