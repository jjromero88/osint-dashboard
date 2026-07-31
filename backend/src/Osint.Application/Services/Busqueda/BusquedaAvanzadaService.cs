using Osint.Application.DTOs;
using Osint.Application.Interfaces;
using Osint.Common.Response;
using Osint.Domain.Entities;
using Osint.Common.Logging;

namespace Osint.Application.Services;

public class BusquedaAvanzadaService : IBusquedaAvanzadaService
{
    private readonly IBusquedaStore _store;
    private readonly IBusquedaQueue _queue;
    private readonly IValidatorService _validatorService;
    private readonly IAppLogger<BusquedaAvanzadaService> _logger;

    public BusquedaAvanzadaService(
        IBusquedaStore store,
        IBusquedaQueue queue,
        IValidatorService validatorService,
        IAppLogger<BusquedaAvanzadaService> logger)
    {
        _store = store;
        _queue = queue;
        _validatorService = validatorService;
        _logger = logger;
    }

    // Arma el lote: una Busqueda por cada valor de usernames/emails/phones/domains,
    // más un aggregate (SpiderFoot) por el primer valor no vacío de cada campo
    // (incluido `names`, que hoy solo alimenta el aggregate — la permutación de
    // Maigret desde nombre está pendiente en el wrapper, ver plan-trabajo.md §3).
    public async Task<ApiResponse<BusquedaAvanzadaResponseDto>> IniciarBusquedaAvanzadaAsync(BusquedaAvanzadaRequestDto dto)
    {
        try
        {
            var validation = await _validatorService.ValidateAsync(dto);
            if (validation.Error)
                return ApiResponse<BusquedaAvanzadaResponseDto>.Fail(validation.Msg, validation.Errors);

            var busquedas = new List<Busqueda>();
            void Agregar(string tipo, string objetivo)
            {
                busquedas.Add(new Busqueda
                {
                    busqueda_id = Guid.NewGuid(),
                    tipo = tipo,
                    objetivo = objetivo,
                    estado = "queued",
                    fecha_inicio = DateTime.UtcNow
                });
            }

            foreach (var u in dto.usernames.Where(v => !string.IsNullOrWhiteSpace(v)))
                Agregar("username", u);
            foreach (var e in dto.emails.Where(v => !string.IsNullOrWhiteSpace(v)))
                Agregar("email", e);
            foreach (var p in dto.phones.Where(v => !string.IsNullOrWhiteSpace(v)))
                Agregar("phone", p);
            foreach (var d in dto.domains.Where(v => !string.IsNullOrWhiteSpace(v)))
                Agregar("domain", d);

            // Pase agregado: solo el primer valor no vacío de cada campo, para no
            // disparar demasiados escaneos lentos de SpiderFoot a la vez.
            var principales = new[]
            {
                dto.usernames.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)),
                dto.emails.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)),
                dto.phones.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)),
                dto.domains.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v)),
                dto.names.FirstOrDefault(v => !string.IsNullOrWhiteSpace(v))
            }.Where(v => v is not null).Cast<string>().Distinct();

            foreach (var principal in principales)
                Agregar("aggregate", principal);

            if (busquedas.Count == 0)
                return ApiResponse<BusquedaAvanzadaResponseDto>.Fail("Debe ingresar al menos un dato para buscar.");

            var lote = new Lote
            {
                lote_id = Guid.NewGuid(),
                estado = "queued",
                busqueda_ids = busquedas.Select(b => b.busqueda_id).ToList(),
                fecha_inicio = DateTime.UtcNow
            };

            foreach (var busqueda in busquedas)
            {
                busqueda.lote_id = lote.lote_id;
                _store.Guardar(busqueda);
            }
            _store.GuardarLote(lote);

            foreach (var busqueda in busquedas)
                await _queue.EncolarAsync(busqueda.busqueda_id);

            return ApiResponse<BusquedaAvanzadaResponseDto>.Ok(
                ConsolidarResultado(lote, busquedas), "Lote de búsqueda encolado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar la búsqueda avanzada.");
            return ApiResponse<BusquedaAvanzadaResponseDto>.Fail("Ocurrió un error al iniciar la búsqueda avanzada.");
        }
    }

    // Junta las señales de todas las búsquedas del lote y las deduplica por source_url
    public Task<ApiResponse<BusquedaAvanzadaResponseDto>> ObtenerBusquedaAvanzadaAsync(Guid loteId)
    {
        try
        {
            var lote = _store.ObtenerLote(loteId);
            if (lote is null)
                return Task.FromResult(ApiResponse<BusquedaAvanzadaResponseDto>.Fail("Lote no encontrado."));

            var busquedas = _store.ObtenerBusquedasDeLote(loteId).ToList();
            return Task.FromResult(ApiResponse<BusquedaAvanzadaResponseDto>.Ok(ConsolidarResultado(lote, busquedas)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la búsqueda avanzada.");
            return Task.FromResult(ApiResponse<BusquedaAvanzadaResponseDto>.Fail("Ocurrió un error al obtener el lote."));
        }
    }

    // Historial en memoria: todos los lotes, cada uno ya consolidado/deduplicado
    public Task<ApiResponse<IEnumerable<BusquedaAvanzadaResponseDto>>> ListarLotesAsync()
    {
        try
        {
            var lotes = _store.ListarLotes()
                .OrderByDescending(l => l.fecha_inicio)
                .Select(lote => ConsolidarResultado(lote, _store.ObtenerBusquedasDeLote(lote.lote_id).ToList()));

            return Task.FromResult(ApiResponse<IEnumerable<BusquedaAvanzadaResponseDto>>.Ok(lotes));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar los lotes de búsqueda avanzada.");
            return Task.FromResult(ApiResponse<IEnumerable<BusquedaAvanzadaResponseDto>>.Fail("Ocurrió un error al listar los lotes."));
        }
    }

    private static BusquedaAvanzadaResponseDto ConsolidarResultado(Lote lote, List<Busqueda> busquedas)
    {
        var estadoLote = busquedas.Count == 0 || busquedas.Any(b => b.estado is "queued" or "running")
            ? "running"
            : "completed";

        // Dedup por source_url normalizado: un hallazgo único, con la lista de qué
        // input/herramienta lo encontró (puede venir de más de una búsqueda).
        var hallazgos = new Dictionary<string, HallazgoDto>(StringComparer.OrdinalIgnoreCase);
        foreach (var busqueda in busquedas)
        {
            foreach (var senal in busqueda.senales)
            {
                var clave = senal.source_url.TrimEnd('/').ToLowerInvariant();
                if (!hallazgos.TryGetValue(clave, out var hallazgo))
                {
                    hallazgo = new HallazgoDto
                    {
                        tipo = senal.tipo,
                        valor = senal.valor,
                        source_url = senal.source_url,
                        confidence = senal.confidence
                    };
                    hallazgos[clave] = hallazgo;
                }
                else if (senal.confidence > hallazgo.confidence)
                {
                    hallazgo.confidence = senal.confidence;
                }

                hallazgo.encontrado_via.Add(new EncontradoViaDto
                {
                    herramienta = senal.herramienta,
                    tipo_input = busqueda.tipo,
                    valor_input = busqueda.objetivo
                });
            }
        }

        var listaHallazgos = hallazgos.Values.ToList();
        var resumen = new ResumenDto
        {
            total_unico = listaHallazgos.Count,
            por_tipo = listaHallazgos.GroupBy(h => h.tipo).ToDictionary(g => g.Key, g => g.Count())
        };

        return new BusquedaAvanzadaResponseDto
        {
            lote_id = lote.lote_id.ToString(),
            estado = estadoLote,
            hallazgos = listaHallazgos,
            resumen = resumen
        };
    }
}
