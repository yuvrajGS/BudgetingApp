from typing import List

from pydantic_settings import BaseSettings, SettingsConfigDict


class Settings(BaseSettings):
    EMBEDDING_MODEL: str = "all-MiniLM-L6-v2"
    CONFIDENCE_THRESHOLD: float = 0.30
    FUZZY_MATCH_THRESHOLD: int = 80
    DATABASE_URL: str = "postgresql+asyncpg://postgres:admin@localhost/BudgetAppDb"

    ALLOWED_ORIGINS: List[str] = [
        "http://localhost:5000",
        "http://localhost:7071",
    ]

    model_config = SettingsConfigDict(
        env_file=".env",
        extra="ignore",
    )


settings = Settings()
