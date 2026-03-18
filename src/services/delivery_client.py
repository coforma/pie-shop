import structlog
import httpx
from src.core.config import settings

logger = structlog.get_logger()


class DeliveryClient:
    def __init__(self):
        self.base_url = settings.delivery_url
        self.api_key = settings.delivery_api_key

    def schedule_delivery(self, order_id: str, address: dict, window_start: str, window_end: str) -> dict:
        try:
            payload = {
                "package": {
                    "orderId": order_id,
                    "size": "medium",
                    "weight": 2.5,
                },
                "destination": {
                    "street": address["street"],
                    "city": address["city"],
                    "state": address["state"],
                    "zip": address["zip"],
                },
                "window": f"{window_start}/{window_end}",
            }

            response = httpx.post(
                f"{self.base_url}/api/v1/deliveries",
                json=payload,
                headers={"X-API-Key": self.api_key},
                timeout=15,
            )
            response.raise_for_status()
            return response.json()

        except httpx.HTTPStatusError as e:
            logger.error("delivery_http_error", status=e.response.status_code, order_id=order_id)
            raise

        except Exception as e:
            logger.error("delivery_error", order_id=order_id, error=str(e))
            raise

    def get_delivery_status(self, delivery_id: str) -> dict:
        response = httpx.get(
            f"{self.base_url}/api/v1/deliveries/{delivery_id}",
            headers={"X-API-Key": self.api_key},
            timeout=10,
        )
        response.raise_for_status()
        return response.json()
