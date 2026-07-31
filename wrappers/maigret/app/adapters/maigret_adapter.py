import asyncio
import json
import os
import shutil
import tempfile


def _find_report(tmp_dir: str, username: str) -> str | None:
    """Maigret guarda el reporte como `report_{username}_ndjson.json`
    (extensión literal .json, no .ndjson)."""
    safe_username = username.replace("/", "_")
    expected = f"report_{safe_username}_ndjson.json"
    candidate = os.path.join(tmp_dir, expected)
    if os.path.exists(candidate):
        return candidate
    for name in os.listdir(tmp_dir):
        if name.startswith("report_") and name.endswith("_ndjson.json"):
            return os.path.join(tmp_dir, name)
    return None


async def run_maigret(
    username: str,
    timeout_seconds: int,
    per_site_timeout_seconds: int,
    top_sites_count: int,
) -> tuple[list[dict], str, bool]:
    """Devuelve (resultados, raw, timed_out).

    Si el proceso se cuelga y hay que matarlo, igual se intenta leer el reporte
    que maigret ya haya alcanzado a escribir en disco antes del kill, en vez de
    descartar resultados parciales.
    """
    tmp_dir = tempfile.mkdtemp(prefix="maigret_")
    timed_out = False
    stdout_data = b""
    stderr_data = b""
    try:
        proc = await asyncio.create_subprocess_exec(
            "maigret",
            username,
            "--json",
            "ndjson",
            "--folderoutput",
            tmp_dir,
            "--timeout",
            str(per_site_timeout_seconds),
            "--top-sites",
            str(top_sites_count),
            # sin recursión/extracción: un escaneo de este wrapper es SOLO sobre
            # el target pedido, no una cascada de escaneos sobre IDs que maigret
            # descubra en los perfiles encontrados (eso multiplica el tiempo de
            # forma no acotada y rompe el timeout del wrapper).
            "--no-recursion",
            "--no-extracting",
            "--no-color",
            "--no-progressbar",
            stdout=asyncio.subprocess.PIPE,
            stderr=asyncio.subprocess.PIPE,
        )
        try:
            stdout_data, stderr_data = await asyncio.wait_for(
                proc.communicate(), timeout=timeout_seconds
            )
        except asyncio.TimeoutError:
            timed_out = True
            proc.kill()
            await proc.wait()

        raw = stdout_data.decode(errors="ignore") + stderr_data.decode(errors="ignore")

        results: list[dict] = []
        report_path = _find_report(tmp_dir, username)
        if report_path:
            with open(report_path, encoding="utf-8") as f:
                for line in f:
                    line = line.strip()
                    if line:
                        results.append(json.loads(line))
        return results, raw, timed_out
    finally:
        shutil.rmtree(tmp_dir, ignore_errors=True)
