import asyncio

import httpx
from holehe.core import get_functions, import_submodules

_functions_cache: list | None = None


def _get_functions() -> list:
    """Carga perezosa de los módulos de holehe (una sola vez por proceso)."""
    global _functions_cache
    if _functions_cache is None:
        modules = import_submodules("holehe.modules")
        _functions_cache = get_functions(modules)
    return _functions_cache


async def _safe_call(fn, email: str, client: httpx.AsyncClient, out: list[dict]) -> None:
    try:
        await fn(email, client, out)
    except Exception:
        # un módulo individual fallando (timeout de un sitio, cambio de API, etc.)
        # no debe tumbar el resto del escaneo
        pass


async def run_holehe(email: str, out: list[dict], timeout_seconds: int) -> None:
    """Corre todos los módulos de holehe contra `email`, acumulando resultados en `out`.

    `out` se muta in-place para que, si el timeout global corta el escaneo,
    los resultados de los módulos que sí alcanzaron a terminar no se pierdan.
    """
    functions = _get_functions()
    async with httpx.AsyncClient() as client:
        tasks = [_safe_call(fn, email, client, out) for fn in functions]
        await asyncio.wait_for(asyncio.gather(*tasks), timeout=timeout_seconds)
