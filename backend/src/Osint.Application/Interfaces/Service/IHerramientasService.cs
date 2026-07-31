using Osint.Application.DTOs;
using Osint.Common.Response;

namespace Osint.Application.Interfaces;

public interface IHerramientasService
{
    // Catálogo de tipos de búsqueda soportados y su herramienta
    Task<ApiResponse<IEnumerable<HerramientaDto>>> ListarAsync();

    // Salud de cada herramienta/wrapper detrás (probe propio por herramienta)
    Task<ApiResponse<Dictionary<string, string>>> ObtenerSaludAsync();
}
