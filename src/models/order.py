import uuid
from datetime import datetime
from typing import Optional
from sqlalchemy import String, DateTime, Text, ForeignKey, func
from sqlalchemy.dialects.postgresql import UUID
from sqlalchemy.orm import Mapped, mapped_column, relationship
from src.database import Base
from src.core.state_machine import OrderState


class Order(Base):
    __tablename__ = "orders"

    id: Mapped[str] = mapped_column(String(36), primary_key=True, default=lambda: f"ord_{uuid.uuid4().hex[:8]}")
    pie_type: Mapped[str] = mapped_column(String(50), nullable=False)
    customer_name: Mapped[str] = mapped_column(String(255), nullable=False)
    customer_email: Mapped[str] = mapped_column(String(255), nullable=False)
    customer_phone: Mapped[Optional[str]] = mapped_column(String(20), nullable=True)
    delivery_street: Mapped[str] = mapped_column(String(255), nullable=False)
    delivery_city: Mapped[str] = mapped_column(String(100), nullable=False)
    delivery_state: Mapped[str] = mapped_column(String(2), nullable=False)
    delivery_zip: Mapped[str] = mapped_column(String(10), nullable=False)
    current_state: Mapped[str] = mapped_column(String(20), default=OrderState.ORDERED.value)
    picker_job_id: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    baker_job_id: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    delivery_id: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    estimated_delivery: Mapped[Optional[datetime]] = mapped_column(DateTime, nullable=True)
    error_message: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    created_at: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    updated_at: Mapped[datetime] = mapped_column(DateTime, server_default=func.now(), onupdate=func.now())

    history: Mapped[list["StateHistory"]] = relationship("StateHistory", back_populates="order", lazy="selectin")


class StateHistory(Base):
    __tablename__ = "state_history"

    id: Mapped[int] = mapped_column(primary_key=True, autoincrement=True)
    order_id: Mapped[str] = mapped_column(String(36), ForeignKey("orders.id"), nullable=False)
    from_state: Mapped[Optional[str]] = mapped_column(String(20), nullable=True)
    to_state: Mapped[str] = mapped_column(String(20), nullable=False)
    timestamp: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    notes: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    error_message: Mapped[Optional[str]] = mapped_column(Text, nullable=True)

    order: Mapped["Order"] = relationship("Order", back_populates="history")
