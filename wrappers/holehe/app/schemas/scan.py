from datetime import datetime
from typing import Any

from pydantic import BaseModel, Field


class ScanRequest(BaseModel):
    target: str = Field(
        ...,
        description="Email a investigar. Se prueba contra ~80 sitios (Instagram, "
        "Spotify, Adobe, Amazon, etc.) usando la función de 'recuperar contraseña' "
        "de cada uno, sin alertar al dueño de la cuenta.",
        examples=["test@gmail.com"],
    )
    options: dict[str, Any] = Field(
        default_factory=dict,
        description="Reservado para futuras opciones del escaneo. Sin uso actual.",
    )


class Signal(BaseModel):
    type: str = Field(description="Tipo de hallazgo. Hoy solo se emite 'account_found'.")
    value: str = Field(description="Dominio del sitio donde se encontró la cuenta.", examples=["instagram.com"])
    source_url: str = Field(description="URL de la fuente del hallazgo, para cadena de custodia.")
    confidence: float = Field(description="Confianza del hallazgo (0-1). Baja si el sitio venía con rate-limit.")
    captured_at: datetime = Field(description="Momento (UTC) en que se capturó la señal.")


class ScanResponse(BaseModel):
    tool: str = Field(description="Nombre de la herramienta que generó la respuesta.", examples=["holehe"])
    target: str = Field(description="El email que se escaneó.")
    status: str = Field(description="'ok' si terminó normal, 'timeout' si se agotó el tiempo límite del escaneo.")
    signals: list[Signal] = Field(description="Cuentas encontradas (solo las que existen: exists=true).")
    raw: str = Field(description="Salida cruda de holehe (todos los sitios probados, existan o no), para auditoría.")
    duration_ms: int = Field(description="Cuánto tardó el escaneo, en milisegundos.")


class HealthResponse(BaseModel):
    status: str = Field(description="'ok' si el wrapper está operativo.")
    tool: str = Field(description="Nombre de la herramienta que expone este wrapper.", examples=["holehe"])
