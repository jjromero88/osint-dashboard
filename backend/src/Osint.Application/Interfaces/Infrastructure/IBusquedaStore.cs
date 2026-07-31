using Osint.Domain.Entities;

namespace Osint.Application.Interfaces;

// Estado de búsquedas/lotes en memoria (sin persistencia SQL por ahora — ver
// .claude/skill-decisions.md). Implementado en Infrastructure.
public interface IBusquedaStore
{
    void Guardar(Busqueda busqueda);
    Busqueda? Obtener(Guid busquedaId);
    IReadOnlyCollection<Busqueda> ListarTodas();

    void GuardarLote(Lote lote);
    Lote? ObtenerLote(Guid loteId);
    IReadOnlyCollection<Lote> ListarLotes();
    IEnumerable<Busqueda> ObtenerBusquedasDeLote(Guid loteId);
}
