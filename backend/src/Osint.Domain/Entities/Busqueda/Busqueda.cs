namespace Osint.Domain.Entities;

// Una búsqueda puntual (modo básico) contra una herramienta.
public class Busqueda
{
    public Guid busqueda_id { get; set; }
    public string tipo { get; set; } = string.Empty;
    public string objetivo { get; set; } = string.Empty;
    public string nivel { get; set; } = "medio";
    public string estado { get; set; } = "queued";
    public List<Senal> senales { get; set; } = [];
    public string? raw { get; set; }
    public int? duration_ms { get; set; }
    public DateTime fecha_inicio { get; set; }
    public DateTime? fecha_fin { get; set; }
    public Guid? lote_id { get; set; }
    public string? error { get; set; }
}
