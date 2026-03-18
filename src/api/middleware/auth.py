from fastapi import Request, Response
from starlette.middleware.base import BaseHTTPMiddleware
import structlog

logger = structlog.get_logger()
class LoggingMiddleware(BaseHTTPMiddleware):
    async def dispatch(self, request: Request, call_next):
        logger.info(
            "http_request",
            method=request.method,
            path=request.url.path,
        )
        response: Response = await call_next(request)
        logger.info(
            "http_response",
            method=request.method,
            path=request.url.path,
            status_code=response.status_code,
        )
        return response
