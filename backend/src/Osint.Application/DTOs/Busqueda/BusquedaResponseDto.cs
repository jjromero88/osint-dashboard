namespace Osint.Application.DTOs;

// GET /api/search/{id} — estado y resultado de una búsqueda (modo básico).
public class BusquedaResponseDto
{
    public string busqueda_id { get; set; } = string.Empty;
    public string tipo { get; set; } = string.Empty;
    public string objetivo { get; set; } = string.Empty;
    public string estado { get; set; } = string.Empty;
    public List<SenalDto> senales { get; set; } = [];
    public string? raw { get; set; }
    public int? duration_ms { get; set; }
    public DateTime fecha_inicio { get; set; }
    public DateTime? fecha_fin { get; set; }
    public string? error { get; set; }
}
