namespace Osint.Domain.Entities;

// Agrupa varias Busquedas del modo avanzado (multi-dato) bajo un mismo lote.
public class Lote
{
    public Guid lote_id { get; set; }
    public string estado { get; set; } = "queued";
    public List<Guid> busqueda_ids { get; set; } = [];
    public DateTime fecha_inicio { get; set; }
    public DateTime? fecha_fin { get; set; }
}
