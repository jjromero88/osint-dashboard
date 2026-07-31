// Mismo contrato que Osint.Application.DTOs.{Senal,BusquedaResponse,BusquedaRequest}Dto en el backend.
export interface Senal {
  tipo: string;
  valor: string;
  source_url: string;
  confidence: number;
  captured_at: string;
  herramienta: string;
}

export interface Busqueda {
  busqueda_id: string;
  tipo: string;
  objetivo: string;
  nivel: string;
  estado: string;
  senales: Senal[];
  raw: string | null;
  duration_ms: number | null;
  fecha_inicio: string;
  fecha_fin: string | null;
  error: string | null;
}

export interface BusquedaRequestDto {
  tipo: string;
  objetivo: string;
  nivel: string;
}
