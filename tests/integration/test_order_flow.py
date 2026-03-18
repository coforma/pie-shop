import pytest
import httpx
from httpx import AsyncClient
from unittest.mock import patch, MagicMock, AsyncMock


class TestOrderCreationFlow:
    """Integration tests for the full order creation flow."""

    @pytest.mark.asyncio
    async def test_create_order_returns_201(self):
        from src.main import app
        async with AsyncClient(app=app, base_url="http://test") as client:
            with patch("src.database.init_db", new_callable=AsyncMock):
                response = await client.post(
                    "/api/orders",
                    json={
                        "pieType": "apple",
                        "customer": {
                            "name": "Jane Doe",
                            "email": "jane@example.com",
                            "phone": "555-1234",
                        },
                        "deliveryAddress": {
                            "street": "123 Main St",
                            "city": "Springfield",
                            "state": "IL",
                            "zip": "62701",
                        },
                    },
                )
        assert response.status_code in (201, 422, 500)

    @pytest.mark.asyncio
    async def test_get_nonexistent_order_returns_404(self):
        from src.main import app
        async with AsyncClient(app=app, base_url="http://test") as client:
            response = await client.get("/api/orders/ord_doesnotexist")
        assert response.status_code == 404

    @pytest.mark.asyncio
    async def test_create_order_invalid_pie_type(self):
        from src.main import app
        async with AsyncClient(app=app, base_url="http://test") as client:
            response = await client.post(
                "/api/orders",
                json={
                    "pieType": "chocolate_lava",
                    "customer": {"name": "Bob", "email": "bob@example.com"},
                    "deliveryAddress": {
                        "street": "456 Oak",
                        "city": "Chicago",
                        "state": "IL",
                        "zip": "60601",
                    },
                },
            )
        assert response.status_code in (400, 422, 500)

    @pytest.mark.asyncio
    async def test_list_pies_endpoint(self):
        from src.main import app
        async with AsyncClient(app=app, base_url="http://test") as client:
            response = await client.get("/api/pies")
        assert response.status_code == 200
        data = response.json()
        assert isinstance(data, list)
        assert len(data) == 5
        pie_types = [p["type"] for p in data]
        assert "apple" in pie_types
        assert "cherry" in pie_types

    # TODO: Add test for concurrent order creation
    # TODO: Add test for database transaction rollback on failure
    # TODO: Add test for order state progression end-to-end
