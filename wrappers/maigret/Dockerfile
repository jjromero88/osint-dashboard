FROM soxoj/maigret:latest

WORKDIR /app

RUN pip install --no-cache-dir fastapi "uvicorn[standard]"

COPY app /app/app

EXPOSE 8002

ENTRYPOINT []
CMD ["uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8002"]
