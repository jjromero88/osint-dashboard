FROM python:3.11-slim

WORKDIR /app

RUN pip install --no-cache-dir holehe fastapi "uvicorn[standard]"

COPY app /app/app

EXPOSE 8003

CMD ["uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8003"]
