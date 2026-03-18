from fastapi import APIRouter
from src.models.recipe import PIE_CATALOG

router = APIRouter(prefix="/api", tags=["catalog"])


@router.get("/pies")
async def list_pies():
    return [
        {
            "type": key,
            "name": val["name"],
            "description": val["description"],
            "difficulty": val["difficulty"],
            "bakingTime": val["bakingTime"],
        }
        for key, val in PIE_CATALOG.items()
    ]


@router.get("/pies/{pie_type}")
async def get_pie(pie_type: str):
    recipe = PIE_CATALOG.get(pie_type)
    if not recipe:
        from fastapi import HTTPException
        raise HTTPException(status_code=404, detail=f"Pie type '{pie_type}' not found")
    return recipe
