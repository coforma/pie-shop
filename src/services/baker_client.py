import time
import structlog
import httpx
from src.core.config import settings

logger = structlog.get_logger()


class BakerClient:
    def __init__(self):
        self.base_url = settings.baker_url
        self.api_key = settings.baker_api_key

    def start_baking(self, pie_type: str, temperature: int, duration: int) -> dict:
        try:
            response = httpx.post(
                f"{self.base_url}/api/v1/bake",
                json={"pieType": pie_type, "temperature": temperature, "duration": duration},
                headers={"X-API-Key": self.api_key},
                timeout=30,
            )
            response.raise_for_status()
            return response.json()

        except httpx.TimeoutException as e:
            logger.error("baker_timeout", error=str(e))
            raise

        except httpx.HTTPStatusError as e:
            logger.error("baker_http_error", status=e.response.status_code, error=str(e))
            raise

        except Exception as e:
            logger.error("baker_unexpected_error", error=str(e))
            raise

    def get_job_status(self, job_id: str) -> dict:
        response = httpx.get(
            f"{self.base_url}/api/v1/jobs/{job_id}",
            headers={"X-API-Key": self.api_key},
            timeout=10,
        )
        response.raise_for_status()
        return response.json()
