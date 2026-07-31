using System.Collections.Concurrent;
using Osint.Application.Interfaces;
using Osint.Domain.Entities;

namespace Osint.Infrastructure.Store;

// Estado en memoria de búsquedas/lotes — sin persistencia SQL por ahora
// (ver .claude/skill-decisions.md). Se pierde si el proceso se reinicia.
public class BusquedaMemoryStore : IBusquedaStore
{
    private readonly ConcurrentDictionary<Guid, Busqueda> _busquedas = new();
    private readonly ConcurrentDictionary<Guid, Lote> _lotes = new();

    public void Guardar(Busqueda busqueda) => _busquedas[busqueda.busqueda_id] = busqueda;

    public Busqueda? Obtener(Guid busquedaId) => _busquedas.GetValueOrDefault(busquedaId);

    public IReadOnlyCollection<Busqueda> ListarTodas() => _busquedas.Values.ToList();

    public void GuardarLote(Lote lote) => _lotes[lote.lote_id] = lote;

    public Lote? ObtenerLote(Guid loteId) => _lotes.GetValueOrDefault(loteId);

    public IReadOnlyCollection<Lote> ListarLotes() => _lotes.Values.ToList();

    public IEnumerable<Busqueda> ObtenerBusquedasDeLote(Guid loteId)
    {
        var lote = ObtenerLote(loteId);
        if (lote is null)
            return [];

        return lote.busqueda_ids
            .Select(Obtener)
            .Where(b => b is not null)
            .Cast<Busqueda>();
    }
}
