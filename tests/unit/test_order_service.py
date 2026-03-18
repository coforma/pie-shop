import pytest
from unittest.mock import AsyncMock, MagicMock, patch
from src.services.order_service import OrderService, OrderNotFoundError, InvalidOrderError
from src.core.state_machine import OrderState
from src.models.order import Order


class TestOrderServiceValidation:
    @pytest.fixture
    def mock_db(self):
        db = AsyncMock()
        db.flush = AsyncMock()
        db.commit = AsyncMock()
        db.refresh = AsyncMock()
        db.add = MagicMock()
        return db

    @pytest.fixture
    def service(self, mock_db):
        return OrderService(mock_db)

    @pytest.mark.asyncio
    async def test_create_order_invalid_pie_type(self, service):
        with pytest.raises(InvalidOrderError) as exc_info:
            await service.create_order(
                pie_type="chocolate",
                customer={"name": "Jane", "email": "jane@example.com"},
                delivery_address={"street": "123 Main", "city": "Springfield", "state": "IL", "zip": "62701"},
            )
        assert "not available" in str(exc_info.value)
        assert "chocolate" in str(exc_info.value)

    @pytest.mark.asyncio
    async def test_create_order_missing_customer_name(self, service):
        with pytest.raises(InvalidOrderError):
            await service.create_order(
                pie_type="apple",
                customer={"name": "", "email": "jane@example.com"},
                delivery_address={"street": "123 Main", "city": "Springfield", "state": "IL", "zip": "62701"},
            )

    @pytest.mark.asyncio
    async def test_create_order_missing_email(self, service):
        with pytest.raises(InvalidOrderError):
            await service.create_order(
                pie_type="apple",
                customer={"name": "Jane Doe", "email": ""},
                delivery_address={"street": "123 Main", "city": "Springfield", "state": "IL", "zip": "62701"},
            )

    @pytest.mark.asyncio
    async def test_list_orders_returns_empty_list(self, service, mock_db):
        mock_result = MagicMock()
        mock_result.scalars.return_value.all.return_value = []
        mock_db.execute = AsyncMock(return_value=mock_result)

        orders = await service.list_orders()
        assert orders == []


class TestOrderServiceCreate:
    @pytest.fixture
    def mock_db(self):
        db = AsyncMock()
        db.flush = AsyncMock()
        db.commit = AsyncMock()
        db.add = MagicMock()
        return db

    @pytest.fixture
    def service(self, mock_db):
        return OrderService(mock_db)

    @pytest.mark.asyncio
    async def test_create_valid_order(self, service, mock_db):
        created_order = None

        def capture_add(obj):
            nonlocal created_order
            if isinstance(obj, Order):
                created_order = obj

        mock_db.add.side_effect = capture_add

        async def mock_refresh(obj):
            obj.id = "ord_test123"
            obj.created_at = MagicMock()
            obj.updated_at = MagicMock()
            obj.history = []

        mock_db.refresh = mock_refresh

        order = await service.create_order(
            pie_type="apple",
            customer={"name": "Jane Doe", "email": "jane@example.com", "phone": "555-1234"},
            delivery_address={"street": "123 Main", "city": "Springfield", "state": "IL", "zip": "62701"},
        )

        assert created_order is not None
        assert created_order.pie_type == "apple"
        assert created_order.customer_name == "Jane Doe"
        assert created_order.current_state == OrderState.ORDERED.value
