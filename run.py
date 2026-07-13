"""
Run with:  uvicorn run:app --host 0.0.0.0 --port 8000 --reload
python -m uvicorn run:app --host 0.0.0.0 --port 8000 --reload
"""
from merchant_categorizer.main import app

if __name__ == "__main__":
    import uvicorn

    uvicorn.run(app, host="0.0.0.0", port=8000)
