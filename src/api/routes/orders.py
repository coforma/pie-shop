from datetime import datetime
from typing import Optional, List
from fastapi import APIRouter, Depends, HTTPException, Query
from sqlalchemy.ext.asyncio import AsyncSession
import structlog

from src.database import get_db
from src.services.order_service import OrderService, OrderNotFoundError, InvalidOrderError
from src.api.schemas import CreateOrderRequest, OrderResponse, OrderSummary, StateHistoryEntry
from src.models.order import Order

router = APIRouter(prefix="/api/orders", tags=["orders"])
logger = structlog.get_logger()

def order_to_response(order: Order) -> dict:
    history = []
    for h in (order.history or []):
        history.append({
            "state": h.to_state,
            "timestamp": h.timestamp,
            "notes": h.notes,
            "error_message": h.error_message,
        })

    return {
        "orderId": order.id,
        "pieType": order.pie_type,
        "status": order.current_state,
        "customer": {
            "name": order.customer_name,
            "email": order.customer_email,
            "phone": order.customer_phone,
        },
        "deliveryAddress": {
            "street": order.delivery_street,
            "city": order.delivery_city,
            "state": order.delivery_state,
            "zip": order.delivery_zip,
        },
        "estimatedDelivery": order.estimated_delivery,
        "createdAt": order.created_at,
        "updatedAt": order.updated_at,
        "history": history,
    }


@router.post("", status_code=201)
async def create_order(request: CreateOrderRequest, db: AsyncSession = Depends(get_db)):
    service = OrderService(db)
    try:
        order = await service.create_order(
            pie_type=request.pieType,
            customer=request.customer.model_dump(),
            delivery_address=request.deliveryAddress.model_dump(),
        )
        return order_to_response(order)
    except InvalidOrderError as e:
        raise HTTPException(status_code=400, detail={"error": "INVALID_ORDER", "message": str(e)})
    except Exception as e:
        logger.error("create_order_failed", error=str(e))
        raise HTTPException(status_code=500, detail={"error": "INTERNAL_ERROR", "message": "Failed to create order"})


@router.get("")
async def list_orders(
    status: Optional[str] = Query(None),
    pie_type: Optional[str] = Query(None),
    db: AsyncSession = Depends(get_db),
):
    service = OrderService(db)
    orders = await service.list_orders(status=status, pie_type=pie_type)
    return [
        {
            "orderId": o.id,
            "pieType": o.pie_type,
            "customerName": o.customer_name,
            "status": o.current_state,
            "createdAt": o.created_at,
            "estimatedDelivery": o.estimated_delivery,
        }
        for o in orders
    ]


@router.get("/{order_id}")
async def get_order(order_id: str, db: AsyncSession = Depends(get_db)):
    service = OrderService(db)
    try:
        order = await service.get_order(order_id)
        return order_to_response(order)
    except OrderNotFoundError as e:
        raise HTTPException(status_code=404, detail={"error": "ORDER_NOT_FOUND", "message": str(e)})


@router.post("/{order_id}/advance")
async def advance_order(order_id: str, db: AsyncSession = Depends(get_db)):
    service = OrderService(db)
    try:
        order = await service.advance_order(order_id)
        return order_to_response(order)
    except OrderNotFoundError as e:
        raise HTTPException(status_code=404, detail={"error": "ORDER_NOT_FOUND", "message": str(e)})
    except InvalidOrderError as e:
        raise HTTPException(status_code=400, detail={"error": "INVALID_TRANSITION", "message": str(e)})
    except Exception as e:
        logger.error("advance_order_failed", order_id=order_id, error=str(e))
        raise HTTPException(status_code=500, detail={"error": "INTERNAL_ERROR", "message": str(e)})


@router.patch("/{order_id}")
async def update_order(order_id: str, updates: dict, db: AsyncSession = Depends(get_db)):
    # TODO: Add authorization check - admin only
    service = OrderService(db)
    try:
        order = await service.get_order(order_id)
        return order_to_response(order)
    except OrderNotFoundError as e:
        raise HTTPException(status_code=404, detail={"error": "ORDER_NOT_FOUND", "message": str(e)})
