// Mismo contrato que Osint.Application.DTOs.{EncontradoVia,Hallazgo,Resumen,BusquedaAvanzada*}Dto en el backend.
export interface EncontradoVia {
  herramienta: string;
  tipo_input: string;
  valor_input: string;
}

export interface Hallazgo {
  tipo: string;
  valor: string;
  source_url: string;
  confidence: number;
  encontrado_via: EncontradoVia[];
}

export interface Resumen {
  total_unico: number;
  por_tipo: Record<string, number>;
}

export interface BusquedaAvanzada {
  lote_id: string;
  estado: string;
  hallazgos: Hallazgo[];
  resumen: Resumen;
}

export interface BusquedaAvanzadaRequestDto {
  usernames: string[];
  emails: string[];
  phones: string[];
  domains: string[];
  names: string[];
}
