using AutoMapper;
using Osint.Application.DTOs;
using Osint.Application.Interfaces;
using Osint.Common.Response;
using Osint.Domain.Entities;
using Osint.Common.Logging;

namespace Osint.Application.Services;

public class BusquedaService : IBusquedaService
{
    private readonly IReadOnlyDictionary<string, IOsintToolClient> _toolClients;
    private readonly IBusquedaStore _store;
    private readonly IBusquedaQueue _queue;
    private readonly IValidatorService _validatorService;
    private readonly IMapper _mapper;
    private readonly IAppLogger<BusquedaService> _logger;

    public BusquedaService(
        IEnumerable<IOsintToolClient> toolClients,
        IBusquedaStore store,
        IBusquedaQueue queue,
        IValidatorService validatorService,
        IMapper mapper,
        IAppLogger<BusquedaService> logger)
    {
        _toolClients = toolClients.ToDictionary(c => c.Tipo, c => c);
        _store = store;
        _queue = queue;
        _validatorService = validatorService;
        _mapper = mapper;
        _logger = logger;
    }

    // Valida el tipo/objetivo, crea la búsqueda en estado queued y la encola
    public async Task<ApiResponse<BusquedaResponseDto>> IniciarBusquedaAsync(BusquedaRequestDto dto)
    {
        try
        {
            var validation = await _validatorService.ValidateAsync(dto);
            if (validation.Error)
                return ApiResponse<BusquedaResponseDto>.Fail(validation.Msg, validation.Errors);

            if (!_toolClients.TryGetValue(dto.tipo, out var toolClient))
                return ApiResponse<BusquedaResponseDto>.Fail($"Tipo de búsqueda no soportado: '{dto.tipo}'.");

            var busqueda = new Busqueda
            {
                busqueda_id = Guid.NewGuid(),
                tipo = dto.tipo,
                objetivo = dto.objetivo,
                estado = "queued",
                fecha_inicio = DateTime.UtcNow
            };

            _store.Guardar(busqueda);
            await _queue.EncolarAsync(busqueda.busqueda_id);

            return ApiResponse<BusquedaResponseDto>.Ok(_mapper.Map<BusquedaResponseDto>(busqueda), "Búsqueda encolada.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar la búsqueda.");
            return ApiResponse<BusquedaResponseDto>.Fail("Ocurrió un error al iniciar la búsqueda.");
        }
    }

    // Estado/resultado de una búsqueda por su id
    public Task<ApiResponse<BusquedaResponseDto>> ObtenerBusquedaAsync(Guid busquedaId)
    {
        try
        {
            var busqueda = _store.Obtener(busquedaId);
            if (busqueda is null)
                return Task.FromResult(ApiResponse<BusquedaResponseDto>.Fail("Búsqueda no encontrada."));

            return Task.FromResult(ApiResponse<BusquedaResponseDto>.Ok(_mapper.Map<BusquedaResponseDto>(busqueda)));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener la búsqueda.");
            return Task.FromResult(ApiResponse<BusquedaResponseDto>.Fail("Ocurrió un error al obtener la búsqueda."));
        }
    }

    // Historial en memoria (vive mientras el proceso esté arriba, sin persistencia)
    public Task<ApiResponse<IEnumerable<BusquedaResponseDto>>> ListarBusquedasAsync()
    {
        try
        {
            var busquedas = _store.ListarTodas()
                .OrderByDescending(b => b.fecha_inicio)
                .Select(_mapper.Map<BusquedaResponseDto>);

            return Task.FromResult(ApiResponse<IEnumerable<BusquedaResponseDto>>.Ok(busquedas));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al listar las búsquedas.");
            return Task.FromResult(ApiResponse<IEnumerable<BusquedaResponseDto>>.Fail("Ocurrió un error al listar las búsquedas."));
        }
    }
}
