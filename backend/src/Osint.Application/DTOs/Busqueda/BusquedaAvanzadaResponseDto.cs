namespace Osint.Application.DTOs;

// GET /api/search/advanced/{batchId} — resultado consolidado del lote.
public class BusquedaAvanzadaResponseDto
{
    public string lote_id { get; set; } = string.Empty;
    public string estado { get; set; } = string.Empty;
    public List<HallazgoDto> hallazgos { get; set; } = [];
    public ResumenDto resumen { get; set; } = new();
}
