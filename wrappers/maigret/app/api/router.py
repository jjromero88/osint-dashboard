from fastapi import APIRouter

from app.core.config import SCAN_TIMEOUT_SECONDS, TOOL_NAME, TOP_SITES_COUNT
from app.schemas.scan import HealthResponse, ScanRequest, ScanResponse
from app.services.scan_service import perform_scan

router = APIRouter()


@router.post(
    "/scan",
    response_model=ScanResponse,
    summary=f"Buscar un username en los top {TOP_SITES_COUNT} sitios",
    description=(
        "Dado un **username**, lo busca en los sitios más populares "
        "(configurable, hoy top "
        f"{TOP_SITES_COUNT}) y devuelve solo las coincidencias confirmadas, "
        "con la URL directa de cada perfil.\n\n"
        "Corre con recursión desactivada a propósito: Maigret por defecto "
        "persigue IDs que descubre en cada perfil y encadena escaneos nuevos "
        "sobre ellos, lo que puede tardar minutos sin cota. Acá un `/scan` es "
        "siempre solo sobre el `target` pedido.\n\n"
        f"El escaneo corta a los **{SCAN_TIMEOUT_SECONDS}s** (`status: timeout`); "
        "si eso pasa, igual devuelve lo que Maigret ya haya escrito a disco "
        "antes de matar el proceso."
    ),
)
async def scan(request: ScanRequest) -> ScanResponse:
    return await perform_scan(request.target)


@router.get(
    "/health",
    response_model=HealthResponse,
    summary="Estado del wrapper",
    description="Chequeo simple de disponibilidad, usado por Docker healthcheck y por el backend orquestador.",
)
async def health() -> HealthResponse:
    return HealthResponse(status="ok", tool=TOOL_NAME)
