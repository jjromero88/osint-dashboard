import time
from datetime import datetime, timezone

from app.adapters.maigret_adapter import run_maigret
from app.core.config import (
    PER_SITE_TIMEOUT_SECONDS,
    SCAN_TIMEOUT_SECONDS,
    TOOL_NAME,
    TOP_SITES_COUNT,
)
from app.schemas.scan import ScanResponse, Signal


async def perform_scan(target: str) -> ScanResponse:
    start = time.monotonic()

    results, raw, timed_out = await run_maigret(
        target,
        timeout_seconds=SCAN_TIMEOUT_SECONDS,
        per_site_timeout_seconds=PER_SITE_TIMEOUT_SECONDS,
        top_sites_count=TOP_SITES_COUNT,
    )
    # aunque haya habido timeout y se haya matado el proceso, `results` puede
    # traer lo que maigret ya alcanzó a escribir en disco antes del kill.
    status = "timeout" if timed_out else "ok"

    captured_at = datetime.now(timezone.utc)
    signals: list[Signal] = []
    # cada línea del .ndjson que genera maigret ya viene filtrada a coincidencias
    # confirmadas (status == CLAIMED); no hace falta re-filtrar acá.
    for item in results:
        status_info = item.get("status") or {}
        site_name = status_info.get("site_name") or item.get("sitename") or "unknown"
        url_user = item.get("url_user") or status_info.get("url")
        if not url_user:
            continue
        signals.append(
            Signal(
                type="account_found",
                value=site_name,
                source_url=url_user,
                confidence=0.85,
                captured_at=captured_at,
            )
        )

    duration_ms = int((time.monotonic() - start) * 1000)
    return ScanResponse(
        tool=TOOL_NAME,
        target=target,
        status=status,
        signals=signals,
        raw=raw[:20000],
        duration_ms=duration_ms,
    )
