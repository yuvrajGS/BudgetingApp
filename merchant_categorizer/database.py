"""
Database layer — READ ONLY from the Python side.
Tables (categories, merchant_aliases) are created and owned by the C# backend via EF Core.
This module only provides a session factory and ORM models that map to those existing tables.
"""

from sqlalchemy.ext.asyncio import AsyncSession, create_async_engine, async_sessionmaker
from sqlalchemy.orm import DeclarativeBase, Mapped, mapped_column
from sqlalchemy import String, Text, DateTime, func
from datetime import datetime
from typing import AsyncGenerator

from .config import settings

engine = create_async_engine(settings.DATABASE_URL, echo=False)
AsyncSessionLocal = async_sessionmaker(engine, expire_on_commit=False)


class Base(DeclarativeBase):
    pass


class Category(Base):
    """
    Maps to the 'categories' table created by C# EF Core migrations.
    'keywords' is a comma-separated hint list that enriches embeddings,
    e.g. "groceries,supermarket,food store" for a "Groceries" category.
    """
    __tablename__ = "Categories"

    id: Mapped[int] = mapped_column("Id",primary_key=True, index=True)
    name: Mapped[str] = mapped_column("Name",String(100), unique=True, nullable=False)
    description: Mapped[str] = mapped_column("Description",Text, nullable=True)
    keywords: Mapped[str] = mapped_column("Keywords",Text, nullable=True)  # comma-separated
    created_at: Mapped[datetime] = mapped_column("CreatedAt",
        DateTime, server_default=func.now(), onupdate=func.now()
    )


class MerchantAlias(Base):
    """
    Maps to the 'merchant_aliases' table created by C# EF Core migrations.
    Maps raw merchant strings (e.g. 'AMZN MKTP US') to categories ('Shopping').
    Populated by the C# backend as it learns corrections from users.
    """
    __tablename__ = "MerchantAlias"

    id: Mapped[int] = mapped_column("Id",primary_key=True, index=True)
    raw_name: Mapped[str] = mapped_column("RawName",String(200), unique=True, index=True)
    category: Mapped[str] = mapped_column("Category",String(200))


async def get_db() -> AsyncGenerator[AsyncSession, None]:
    async with AsyncSessionLocal() as session:
        yield session