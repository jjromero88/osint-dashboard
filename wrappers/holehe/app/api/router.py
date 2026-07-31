from fastapi import APIRouter

from app.core.config import SCAN_TIMEOUT_SECONDS, TOOL_NAME
from app.schemas.scan import HealthResponse, ScanRequest, ScanResponse
from app.services.scan_service import perform_scan

router = APIRouter()


@router.post(
    "/scan",
    response_model=ScanResponse,
    summary="Buscar un email en ~80 sitios",
    description=(
        "Dado un **email**, revisa en paralelo un conjunto de sitios (redes "
        "sociales, e-commerce, foros...) usando la función de 'recuperar "
        "contraseña' de cada uno, para saber si el email está registrado ahí — "
        "sin alertar al dueño de la cuenta.\n\n"
        f"El escaneo corta a los **{SCAN_TIMEOUT_SECONDS}s** (`status: timeout`); "
        "si eso pasa, igual devuelve lo que ya se alcanzó a detectar."
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
