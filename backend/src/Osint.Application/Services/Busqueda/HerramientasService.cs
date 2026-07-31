using Osint.Application.Common;
using Osint.Application.DTOs;
using Osint.Application.Interfaces;
using Osint.Common.Response;
using Osint.Common.Logging;

namespace Osint.Application.Services;

public class HerramientasService : IHerramientasService
{
    private readonly IReadOnlyCollection<IOsintToolClient> _toolClients;
    private readonly IAppLogger<HerramientasService> _logger;

    public HerramientasService(IEnumerable<IOsintToolClient> toolClients, IAppLogger<HerramientasService> logger)
    {
        _toolClients = toolClients.ToList();
        _logger = logger;
    }

    // Catálogo tipo -> herramienta + descripción legible (para el <select> del front),
    // cruzando el catálogo estático de tipos con los clientes registrados.
    public Task<ApiResponse<IEnumerable<HerramientaDto>>> ListarAsync()
    {
        var herramientaPorTipo = _toolClients.ToDictionary(c => c.Tipo, c => c.Herramienta);

        var catalogo = CatalogoTipos.Tipos.Select(t => new HerramientaDto
        {
            tipo = t.Value,
            descripcion = t.Desc,
            herramienta = herramientaPorTipo.GetValueOrDefault(t.Value, string.Empty)
        });

        return Task.FromResult(ApiResponse<IEnumerable<HerramientaDto>>.Ok(catalogo));
    }

    // Consulta el probe de salud de cada herramienta en paralelo
    public async Task<ApiResponse<Dictionary<string, string>>> ObtenerSaludAsync()
    {
        try
        {
            var chequeos = await Task.WhenAll(_toolClients.DistinctBy(c => c.Herramienta).Select(async c =>
            {
                var saludable = await c.EstaSaludableAsync(CancellationToken.None);
                return (c.Herramienta, saludable);
            }));

            var salud = chequeos.ToDictionary(x => x.Herramienta, x => x.saludable ? "ok" : "down");
            return ApiResponse<Dictionary<string, string>>.Ok(salud);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al consultar la salud de las herramientas.");
            return ApiResponse<Dictionary<string, string>>.Fail("Ocurrió un error al consultar la salud de las herramientas.");
        }
    }
}
