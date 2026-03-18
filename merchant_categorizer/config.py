from pydantic_settings import BaseSettings
from typing import List


class Settings(BaseSettings):
    # Model
    EMBEDDING_MODEL: str = "all-MiniLM-L6-v2"

    # Minimum cosine similarity to accept a category match (0–1).
    # Below this → returns "Uncategorized".
    CONFIDENCE_THRESHOLD: float = 0.30

    # Fuzzy match score (0–100) required to trigger merchant name normalization.
    FUZZY_MATCH_THRESHOLD: int = 80

    # Database URL
    DATABASE_URL: str = "postgresql+asyncpg://postgres:admin@localhost/BudgetAppDb"

    ALLOWED_ORIGINS: List[str] = ["http://localhost:5000", "http://localhost:7071"]

    class Config:
        env_file = ".env"


settings = Settings()