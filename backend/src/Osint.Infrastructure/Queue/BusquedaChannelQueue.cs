using System.Threading.Channels;
using Osint.Application.Interfaces;

namespace Osint.Infrastructure.Queue;

// Cola en memoria (sin Redis): el endpoint encola y responde al instante,
// BusquedaWorker (BackgroundService) la procesa por detrás.
public class BusquedaChannelQueue : IBusquedaQueue
{
    private readonly Channel<Guid> _channel = Channel.CreateUnbounded<Guid>();

    public ValueTask EncolarAsync(Guid busquedaId, CancellationToken cancellationToken = default)
        => _channel.Writer.WriteAsync(busquedaId, cancellationToken);

    public IAsyncEnumerable<Guid> LeerTodoAsync(CancellationToken cancellationToken)
        => _channel.Reader.ReadAllAsync(cancellationToken);
}
