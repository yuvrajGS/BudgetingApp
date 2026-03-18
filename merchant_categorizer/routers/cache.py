"""
Cache management
The C# backend calls POST /api/v1/cache/invalidate after any category or alias change,
triggering a re-embed on the next categorization request.
"""
 
from fastapi import APIRouter, Request
 
router = APIRouter()
 
 
@router.post(
    "/cache/invalidate",
    summary="Invalidate the category embedding cache",
    description="Call this from the C# backend after adding, updating, or deleting a category or merchant alias.",
)
async def invalidate_cache(request: Request):
    await request.app.state.embedding_service.invalidate_cache()
    return {"status": "cache invalidated"}