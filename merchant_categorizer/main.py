"""
Merchant Categorization API
Uses sentence-transformer embeddings + cosine similarity for zero-shot classification.
Category embeddings are cached in memory and refreshed only when categories change.
"""
import uvicorn
from fastapi import FastAPI, HTTPException, Depends
from fastapi.middleware.cors import CORSMiddleware
from contextlib import asynccontextmanager
import logging

from .routers import cache, categorize
from .services.embedding_service import EmbeddingService
from .config import settings
# NOTE: No DB init here — tables are created/migrated by the C# backend (EF Core)

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)


@asynccontextmanager
async def lifespan(app: FastAPI):
    """Startup: warm up the model and cache embeddings."""
    logger.info("Loading sentence transformer model...")
    embedding_service = EmbeddingService()
    await embedding_service.initialize()
    app.state.embedding_service = embedding_service
    logger.info("Model ready. API is live.")
    yield
    logger.info("Shutting down.")


app = FastAPI(
    title="Merchant Categorization API",
    description="ML-powered merchant name → budget category classifier",
    version="1.0.0",
    lifespan=lifespan,
)

app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.ALLOWED_ORIGINS,
    allow_methods=["*"],
    allow_headers=["*"],
)

app.include_router(categorize.router, prefix="/api/v1", tags=["Categorization"])
app.include_router(cache.router, prefix="/api/v1", tags=["Cache"])


@app.get("/health")
async def health():
    return {"status": "ok"}