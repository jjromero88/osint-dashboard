using Osint.Application.DTOs;
using Osint.Common.Response;

namespace Osint.Application.Interfaces;

public interface IBusquedaService
{
    // Crea la búsqueda, la encola y responde de inmediato (estado queued)
    Task<ApiResponse<BusquedaResponseDto>> IniciarBusquedaAsync(BusquedaRequestDto dto);

    // Estado/resultado de una búsqueda puntual
    Task<ApiResponse<BusquedaResponseDto>> ObtenerBusquedaAsync(Guid busquedaId);

    // Historial en memoria de todas las búsquedas de esta sesión del proceso
    Task<ApiResponse<IEnumerable<BusquedaResponseDto>>> ListarBusquedasAsync();
}
