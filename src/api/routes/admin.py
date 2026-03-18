from fastapi import APIRouter, Depends
from sqlalchemy.ext.asyncio import AsyncSession
from src.database import get_db
from src.services.order_service import OrderService

router = APIRouter(prefix="/api/admin", tags=["admin"])

@router.get("/stats")
async def get_stats(db: AsyncSession = Depends(get_db)):
    service = OrderService(db)
    counts = await service.get_order_count_by_status()
    return {
        "ordersByStatus": counts,
        "total": sum(counts.values()),
    }
