from datetime import datetime
from typing import Any

from pydantic import BaseModel, Field


class ScanRequest(BaseModel):
    target: str = Field(
        ...,
        description="Username a investigar. Se busca en los ~300 sitios más "
        "populares (redes sociales, foros, juegos, sitios de nicho...) para "
        "ver dónde existe una cuenta con ese mismo nombre de usuario.",
        examples=["mrrobot"],
    )
    options: dict[str, Any] = Field(
        default_factory=dict,
        description="Reservado para futuras opciones del escaneo. Sin uso actual.",
    )


class Signal(BaseModel):
    type: str = Field(description="Tipo de hallazgo. Hoy solo se emite 'account_found'.")
    value: str = Field(description="Nombre del sitio donde se encontró la cuenta.", examples=["Instagram"])
    source_url: str = Field(description="URL directa al perfil encontrado, para cadena de custodia.")
    confidence: float = Field(
        description="Confianza del hallazgo (0-1). Fija en 0.85: Maigret ya filtra solo coincidencias confirmadas."
    )
    captured_at: datetime = Field(description="Momento (UTC) en que se capturó la señal.")


class ScanResponse(BaseModel):
    tool: str = Field(description="Nombre de la herramienta que generó la respuesta.", examples=["maigret"])
    target: str = Field(description="El username que se escaneó.")
    status: str = Field(description="'ok' si terminó normal, 'timeout' si se agotó el tiempo límite del escaneo.")
    signals: list[Signal] = Field(description="Cuentas encontradas (todas ya confirmadas por Maigret).")
    raw: str = Field(description="Salida cruda del proceso de Maigret (log de progreso), truncada, para auditoría.")
    duration_ms: int = Field(description="Cuánto tardó el escaneo, en milisegundos.")


class HealthResponse(BaseModel):
    status: str = Field(description="'ok' si el wrapper está operativo.")
    tool: str = Field(description="Nombre de la herramienta que expone este wrapper.", examples=["maigret"])
