from fastapi import APIRouter, Request, HTTPException
from pydantic import BaseModel, Field, field_validator
from typing import Literal

router = APIRouter()


class PredictRequest(BaseModel):
    merchant: str = Field(..., min_length=1, max_length=300)

    @field_validator("merchant")
    @classmethod
    def strip_whitespace(cls, v: str) -> str:
        return v.strip()


class PredictionResponse(BaseModel):
    category: str
    confidence: float
    merchant_clean: str
    alias_source: Literal["model", "cache", "fuzzy_cache"]


class BatchPredictRequest(BaseModel):
    merchants: list[str] = Field(..., min_length=1, max_length=500)

    @field_validator("merchants")
    @classmethod
    def strip_all(cls, v: list[str]) -> list[str]:
        return [m.strip() for m in v if m.strip()]


class BatchPredictionResponse(BaseModel):
    categories: list[str]
    details: list[PredictionResponse]


@router.post("/predict", response_model=PredictionResponse)
async def predict(body: PredictRequest, request: Request):
    svc = request.app.state.embedding_service
    r = await svc.categorize(body.merchant)
    return PredictionResponse(
        category=r["category"],
        confidence=r["confidence"],
        merchant_clean=r["merchant_clean"],
        alias_source=r["alias_source"],
    )


@router.post("/predict/batch", response_model=BatchPredictionResponse)
async def predict_batch(body: BatchPredictRequest, request: Request):
    if not body.merchants:
        raise HTTPException(status_code=422, detail="merchants list is empty")
    svc = request.app.state.embedding_service
    results = await svc.categorize_batch(body.merchants)
    return BatchPredictionResponse(
        categories=[r["category"] for r in results],
        details=[
            PredictionResponse(
                category=r["category"],
                confidence=r["confidence"],
                merchant_clean=r["merchant_clean"],
                alias_source=r["alias_source"],
            )
            for r in results
        ],
    )