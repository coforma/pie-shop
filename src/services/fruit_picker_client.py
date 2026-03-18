import time
import structlog
import httpx
from src.core.config import settings

logger = structlog.get_logger()


class FruitPickerClient:
    def __init__(self):
        self.base_url = settings.fruit_picker_url
        self.api_key = settings.fruit_picker_api_key

    def pick_fruit(self, fruit_type: str, quantity: int, quality: str = "premium") -> dict:
        attempt = 0
        last_error = None

        while attempt < settings.max_retries:
            try:
                response = httpx.post(
                    f"{self.base_url}/api/v1/pick-fruit",
                    json={"fruitType": fruit_type, "quantity": quantity, "quality": quality},
                    headers={"X-API-Key": self.api_key},
                    timeout=10,
                )
                response.raise_for_status()
                return response.json()

            except httpx.TimeoutException as e:
                logger.error("fruit_picker_timeout", attempt=attempt, error=str(e))
                last_error = e
                attempt += 1
                # TODO: Implement circuit breaker pattern here
                time.sleep(settings.retry_delay)

            except httpx.HTTPStatusError as e:
                logger.error("fruit_picker_error", status=e.response.status_code, error=str(e))
                raise

        raise Exception(f"Fruit picker failed after {settings.max_retries} attempts: {last_error}")

    def get_job_status(self, job_id: str) -> dict:
        response = httpx.get(
            f"{self.base_url}/api/v1/jobs/{job_id}",
            headers={"X-API-Key": self.api_key},
            timeout=10,
        )
        response.raise_for_status()
        return response.json()
