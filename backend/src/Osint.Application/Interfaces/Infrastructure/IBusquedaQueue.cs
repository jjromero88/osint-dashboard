namespace Osint.Application.Interfaces;

// Cola asíncrona en memoria (BackgroundService + Channel<T> en Infrastructure).
// Los escaneos son lentos (segundos a minutos): el endpoint encola y responde
// al instante; un worker interno procesa cada búsqueda.
public interface IBusquedaQueue
{
    ValueTask EncolarAsync(Guid busquedaId, CancellationToken cancellationToken = default);
    IAsyncEnumerable<Guid> LeerTodoAsync(CancellationToken cancellationToken);
}
