import asyncio
import time
from datetime import datetime, timezone

from app.adapters.holehe_adapter import run_holehe
from app.core.config import SCAN_TIMEOUT_SECONDS, TOOL_NAME
from app.schemas.scan import ScanResponse, Signal


async def perform_scan(target: str) -> ScanResponse:
    start = time.monotonic()
    status = "ok"
    out: list[dict] = []

    try:
        await run_holehe(target, out, SCAN_TIMEOUT_SECONDS)
    except asyncio.TimeoutError:
        status = "timeout"

    captured_at = datetime.now(timezone.utc)
    signals: list[Signal] = []
    for item in out:
        if item.get("exists"):
            domain = item.get("domain") or item.get("name")
            signals.append(
                Signal(
                    type="account_found",
                    value=domain,
                    source_url=f"https://{domain}",
                    confidence=0.9 if not item.get("rateLimit") else 0.5,
                    captured_at=captured_at,
                )
            )

    duration_ms = int((time.monotonic() - start) * 1000)
    return ScanResponse(
        tool=TOOL_NAME,
        target=target,
        status=status,
        signals=signals,
        raw=str(out),
        duration_ms=duration_ms,
    )
