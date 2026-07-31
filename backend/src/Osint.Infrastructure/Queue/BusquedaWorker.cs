using Microsoft.Extensions.Hosting;
using Osint.Application.Interfaces;
using Osint.Common.Logging;

namespace Osint.Infrastructure.Queue;

// Worker que toma cada búsqueda encolada, llama a la herramienta correspondiente
// según el tipo, normaliza el resultado y lo guarda de vuelta en el store.
public class BusquedaWorker : BackgroundService
{
    private readonly IBusquedaQueue _queue;
    private readonly IBusquedaStore _store;
    private readonly IReadOnlyDictionary<string, IOsintToolClient> _toolClients;
    private readonly IAppLogger<BusquedaWorker> _logger;

    public BusquedaWorker(
        IBusquedaQueue queue,
        IBusquedaStore store,
        IEnumerable<IOsintToolClient> toolClients,
        IAppLogger<BusquedaWorker> logger)
    {
        _queue = queue;
        _store = store;
        _toolClients = toolClients.ToDictionary(c => c.Tipo, c => c);
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var busquedaId in _queue.LeerTodoAsync(stoppingToken))
        {
            _ = ProcesarAsync(busquedaId, stoppingToken);
        }
    }

    private async Task ProcesarAsync(Guid busquedaId, CancellationToken stoppingToken)
    {
        var busqueda = _store.Obtener(busquedaId);
        if (busqueda is null)
            return;

        if (!_toolClients.TryGetValue(busqueda.tipo, out var toolClient))
        {
            busqueda.estado = "failed";
            busqueda.error = $"No hay herramienta registrada para el tipo '{busqueda.tipo}'.";
            busqueda.fecha_fin = DateTime.UtcNow;
            _store.Guardar(busqueda);
            return;
        }

        busqueda.estado = "running";
        _store.Guardar(busqueda);

        try
        {
            var resultado = await toolClient.BuscarAsync(busqueda.objetivo, busqueda.nivel, stoppingToken);

            busqueda.estado = resultado.estado;
            busqueda.senales = resultado.senales;
            busqueda.raw = resultado.raw;
            busqueda.duration_ms = resultado.duration_ms;
            busqueda.fecha_fin = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar la búsqueda {BusquedaId} ({Tipo}: {Objetivo}).",
                busquedaId, busqueda.tipo, busqueda.objetivo);
            busqueda.estado = "failed";
            busqueda.error = "Ocurrió un error al ejecutar la herramienta.";
            busqueda.fecha_fin = DateTime.UtcNow;
        }

        _store.Guardar(busqueda);
    }
}
