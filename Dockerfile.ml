# ML Service Dockerfile
FROM python:3.11-slim

WORKDIR /app

# Install system dependencies
RUN apt-get update && apt-get install -y \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Copy requirements and code
COPY requirements.txt .
COPY merchant_categorizer/ ./merchant_categorizer/
COPY run.py .

RUN pip install --no-cache-dir -r requirements.txt

EXPOSE 8000

CMD ["python", "-m", "uvicorn", "run:app", "--host", "0.0.0.0", "--port", "8000"]
