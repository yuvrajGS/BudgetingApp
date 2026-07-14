"""
EmbeddingService — ML classification with a model-skipping result cache.
"""

import logging
import asyncio
from datetime import datetime
from typing import Optional

import numpy as np
from sentence_transformers import SentenceTransformer
from rapidfuzz import fuzz, process as fuzz_process
from sqlalchemy import select, func
from sqlalchemy.dialects.postgresql import insert as pg_insert
from typing import Optional

from ..database import AsyncSessionLocal, Category, MerchantAlias
from ..config import settings
from .normalize_service import normalise

logger = logging.getLogger(__name__)


class EmbeddingService:
    def __init__(self):
        self._model: Optional[SentenceTransformer] = None

        # Category names parallel to rows in _category_matrix
        # e.g. ["Groceries", "Dining", "Transport", ...]
        self._category_names: list[str] = []

        # Shape (N_categories, D) — unit-length embedding per category.
        # None until _refresh_cache() succeeds.
        self._category_matrix: Optional[np.ndarray] = None

        # max(Category.updated_at) at last refresh — detects when categories
        # change so we re-embed them
        self._category_fingerprint: Optional[datetime] = None

        # normalised_merchant_name → category_name
        # Model skip: if a merchant is here, we never embed it.
        # e.g. {"Tim Hortons": "Dining", "Amazon": "Shopping"}
        # Populated from merchant_aliases on startup, updated in-memory on new hits.
        self._cache: dict[str, str] = {}

        # Guards _category_fingerprint to prevent concurrent redundant refreshes.
        # NOT reentrant — never call _refresh_cache() while holding this lock.
        self._lock = asyncio.Lock()

    # ── Public API ────────────────────────────────────────────────────────────

    async def initialize(self):
        self._model = SentenceTransformer(settings.EMBEDDING_MODEL)
        await self._refresh_cache()

    async def categorize(self, merchant_raw: str) -> dict:
        """Classify a single merchant name."""
        await self._ensure_cache_fresh()
        self._raise_if_not_ready()
        return await self._classify_one(merchant_raw)

    async def categorize_batch(self, merchant_raws: list[str]) -> list[dict]:
        """
        Classify a list of merchant names efficiently.

        Cache hits are free — no model involved.
        Only cache misses get embedded, all in one batched model call.
        Results are returned in the same order as the input list.
        """
        await self._ensure_cache_fresh()
        self._raise_if_not_ready()

        # Partition into cache hits and misses.
        # Both exact and fuzzy cache hits bypass the model entirely.
        hits: dict[int, dict] = {}       # index → result (no model needed)
        # original indices of true cache misses
        miss_indices: list[int] = []
        # normalised names that need model classification
        miss_clean: list[str] = []

        for i, raw in enumerate(merchant_raws):
            clean = normalise(raw)

            if clean in self._cache:
                # Exact cache hit
                hits[i] = {
                    "merchant_raw":    raw,
                    "merchant_clean":  clean,
                    "alias_source":    "cache",
                    "category":        self._cache[clean],
                    "confidence":      1.0,
                    "below_threshold": False,
                }
            else:
                # Fuzzy cache hit — "Amazons" close enough to existing "Amazon"
                fuzzy_match = self._fuzzy_cache_lookup(clean)
                if fuzzy_match is not None:
                    matched_key, category = fuzzy_match
                    hits[i] = {
                        "merchant_raw":    raw,
                        "merchant_clean":  matched_key,
                        "alias_source":    "fuzzy_cache",
                        "category":        category,
                        "confidence":      1.0,
                        "below_threshold": False,
                    }
                else:
                    miss_indices.append(i)
                    miss_clean.append(clean)

        # Embed all cache misses in a single model call
        miss_results: list[dict] = []
        if miss_clean:
            if self._model is None:
                raise RuntimeError(
                    "Model not loaded — cannot classify merchants.")

            # (B_miss, D) — only the merchants we haven't seen before
            query_matrix = self._model.encode(
                miss_clean, normalize_embeddings=True, batch_size=64
            )

            if self._category_matrix is None:
                raise RuntimeError(
                    "Category matrix not loaded — cannot classify merchants.")

            # (B_miss, D) @ (D, N_cats) → (B_miss, N_cats)
            # Row i = cosine similarity between miss i and every category
            scores_matrix = query_matrix @ self._category_matrix.T

            for j, original_idx in enumerate(miss_indices):
                raw = merchant_raws[original_idx]
                clean = miss_clean[j]
                scores = scores_matrix[j]
                best_idx = int(np.argmax(scores))
                best_score = float(scores[best_idx])
                below = best_score < settings.CONFIDENCE_THRESHOLD
                category = "Uncategorized" if below else self._category_names[best_idx]

                # Only cache confident results — don't persist "Uncategorized"
                # since the merchant might classify correctly once more categories exist
                if not below:
                    asyncio.create_task(
                        self._persist_cache_entry(clean, category))

                miss_results.append({
                    "merchant_raw":    raw,
                    "merchant_clean":  clean,
                    "alias_source":    "model",
                    "category":        category,
                    "confidence":      round(best_score, 4),
                    "below_threshold": below,
                })

        # Reconstruct results in original input order
        results: list[dict | None] = [None] * len(merchant_raws)
        for i, result in zip(miss_indices, miss_results):
            results[i] = result
        for i, result in hits.items():
            results[i] = result

        return [r for r in results if r is not None]

    async def invalidate_cache(self):
        """
        Force category re-embedding on next request.
        Called by POST /api/v1/cache/invalidate from the C# backend
        after any category change. Does NOT clear the merchant result cache —
        merchant→category mappings remain valid even if category descriptions change.
        """
        async with self._lock:
            self._category_fingerprint = None

    # ── Single merchant pipeline ──────────────────────────────────────────────

    async def _classify_one(self, merchant_raw: str) -> dict:
        """
        Classify one merchant.
          1. Exact cache lookup on normalised name.
          2. Fuzzy cache lookup — catches near-duplicates like "Amazons" vs "Amazon"
             so we don't create redundant cache entries or run the model unnecessarily.
          3. Model classification as last resort.
        """
        clean = normalise(merchant_raw)

        # Exact cache hit
        if clean in self._cache:
            return {
                "merchant_raw":    merchant_raw,
                "merchant_clean":  clean,
                "alias_source":    "cache",
                "category":        self._cache[clean],
                "confidence":      1.0,
                "below_threshold": False,
            }

        # Fuzzy cache hit — reuse existing entry and persist the new key pointing to it
        fuzzy_match = self._fuzzy_cache_lookup(clean)
        if fuzzy_match is not None:
            matched_key, category = fuzzy_match
            return {
                "merchant_raw":    merchant_raw,
                "merchant_clean":  matched_key,
                "alias_source":    "fuzzy_cache",
                "category":        category,
                "confidence":      1.0,
                "below_threshold": False,
            }

        if self._model is None:
            raise RuntimeError("Model not loaded — cannot classify merchant.")

        # Full model classification
        query_vec = self._model.encode([clean], normalize_embeddings=True)
        scores = (self._category_matrix @ query_vec.T).flatten()
        best_idx = int(np.argmax(scores))
        best_score = float(scores[best_idx])
        below = best_score < settings.CONFIDENCE_THRESHOLD
        category = "Uncategorized" if below else self._category_names[best_idx]

        if not below:
            asyncio.create_task(self._persist_cache_entry(clean, category))

        return {
            "merchant_raw":    merchant_raw,
            "merchant_clean":  clean,
            "alias_source":    "model",
            "category":        category,
            "confidence":      round(best_score, 4),
            "below_threshold": below,
        }

    def _fuzzy_cache_lookup(self, clean: str) -> Optional[tuple[str, str]]:
        """
        Check if `clean` is close enough to an existing cache key to be
        considered the same merchant.

        Returns:
            (existing_key, category) if a match is found above the configured
            threshold, otherwise None.
        """
        cache_keys = list(self._cache.keys())
        if not cache_keys:
            return None

        result = fuzz_process.extractOne(
            query=clean,
            choices=cache_keys,
            scorer=fuzz.WRatio,
            score_cutoff=settings.FUZZY_MATCH_THRESHOLD,
        )

        if result is None:
            return None

        matched_key, score, _ = result

        return matched_key, self._cache[matched_key]

    # ── Cache persistence ─────────────────────────────────────────────────────

    async def _persist_cache_entry(self, clean: str, category: str):
        """
        Store normalised_merchant → category in both memory and DB.

        Memory is updated first so subsequent requests in the same process
        get a cache hit without waiting for the DB write.

        The DB write uses INSERT ... ON CONFLICT DO NOTHING so concurrent
        classifications of the same new merchant don't produce duplicate rows
        or errors.
        """
        self._cache[clean] = category  # immediate in-memory update

        try:
            async with AsyncSessionLocal() as session:
                stmt = (
                    pg_insert(MerchantAlias)
                    .values(raw_name=clean, category=category)
                    .on_conflict_do_nothing(index_elements=["RawName"])
                )
                await session.execute(stmt)
                await session.commit()
            logger.debug(f"Cached: '{clean}' -> '{category}'")
        except Exception as e:
            # Non-fatal — in-memory cache still works for this process lifetime
            logger.warning(
                f"Failed to persist cache entry '{clean}' -> '{category}': {e}")

    # ── Cache management ──────────────────────────────────────────────────────

    def _raise_if_not_ready(self):
        """Fail with a clear message if the categories table is still empty."""
        if self._category_matrix is None or not self._category_names:
            raise RuntimeError(
                "No categories loaded. The C# backend must seed the categories "
                "table before classification requests can be served."
            )

    async def _ensure_cache_fresh(self):
        """
        Re-embed categories if the DB fingerprint has changed.
        The lock only guards the fingerprint read/write, NOT the refresh itself,
        because asyncio.Lock is not reentrant and encoding takes time.
        """
        async with AsyncSessionLocal() as session:
            result = await session.execute(select(func.max(Category.created_at)))
            latest: Optional[datetime] = result.scalar()

        if latest is None:
            logger.warning(
                "Categories table is empty — skipping cache refresh.")
            return

        should_refresh = False
        async with self._lock:
            if latest != self._category_fingerprint:
                self._category_fingerprint = latest
                should_refresh = True

        if should_refresh:
            logger.info("Categories changed — refreshing embeddings.")
            await self._refresh_cache()

    async def _refresh_cache(self):
        """
        Reload category embeddings and the merchant result cache from the DB.

        Category embeddings are always rebuilt (descriptions may have changed).
        The merchant result cache (_cache) is loaded from merchant_aliases
        where category is a known category.
        """
        async with AsyncSessionLocal() as session:
            cats = (await session.execute(select(Category))).scalars().all()
            aliases = (await session.execute(select(MerchantAlias))).scalars().all()

        # Enrich each category into a descriptive string for better embedding quality.
        # "Dining. Restaurants, cafes, and takeout. Keywords: restaurant,cafe,pizza"
        # gives the model far more signal than just "Dining".
        texts, names = [], []
        for cat in cats:
            if cat.name == "Uncategorized":
                continue  # never a match target
            parts = [cat.name]
            if cat.description:
                parts.append(cat.description)
            if cat.keywords:
                parts.append(f"Keywords: {cat.keywords}")
            texts.append(". ".join(parts))
            names.append(cat.name)

        if not texts:
            logger.warning("No categories in DB — cache remains empty.")
            return

        if self._model is None:
            raise RuntimeError("Model not loaded — cannot refresh cache.")

        # Encode → (N_cats, D), unit-length for cosine similarity via dot product
        matrix = self._model.encode(
            texts, normalize_embeddings=True, batch_size=64)
        self._category_matrix = np.array(matrix)
        self._category_names = names

        # Load merchant result cache
        category_set = set(names)
        self._cache = {
            a.raw_name: a.category
            for a in aliases
            if a.category in category_set
        }

        logger.info(
            f"Cache refreshed: {len(names)} categories, "
            f"{len(self._cache)} cached merchant classifications."
        )
