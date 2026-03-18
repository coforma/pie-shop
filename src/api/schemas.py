from datetime import datetime
from typing import Optional, List
from pydantic import BaseModel, EmailStr, field_validator


class DeliveryAddress(BaseModel):
    street: str
    city: str
    state: str
    zip: str


class CustomerInfo(BaseModel):
    name: str
    email: str
    phone: Optional[str] = None


class CreateOrderRequest(BaseModel):
    pieType: str
    customer: CustomerInfo
    deliveryAddress: DeliveryAddress


class StateHistoryEntry(BaseModel):
    state: str
    timestamp: datetime
    notes: Optional[str] = None
    error_message: Optional[str] = None

    class Config:
        from_attributes = True


class OrderResponse(BaseModel):
    orderId: str
    pieType: str
    status: str
    customer: CustomerInfo
    deliveryAddress: DeliveryAddress
    estimatedDelivery: Optional[datetime] = None
    createdAt: datetime
    updatedAt: datetime
    history: Optional[List[StateHistoryEntry]] = None

    class Config:
        from_attributes = True


class OrderSummary(BaseModel):
    orderId: str
    pieType: str
    customerName: str
    status: str
    createdAt: datetime
    estimatedDelivery: Optional[datetime] = None

    class Config:
        from_attributes = True
