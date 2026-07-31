using Osint.Application.DTOs;
using Osint.Common.Response;

namespace Osint.Application.Interfaces;

public interface IBusquedaAvanzadaService
{
    // Crea un lote (N búsquedas en paralelo + 1 aggregate por dato principal) y lo encola
    Task<ApiResponse<BusquedaAvanzadaResponseDto>> IniciarBusquedaAvanzadaAsync(BusquedaAvanzadaRequestDto dto);

    // Resultado consolidado y deduplicado del lote
    Task<ApiResponse<BusquedaAvanzadaResponseDto>> ObtenerBusquedaAvanzadaAsync(Guid loteId);

    // Historial en memoria de todos los lotes de esta sesión del proceso, cada uno ya consolidado
    Task<ApiResponse<IEnumerable<BusquedaAvanzadaResponseDto>>> ListarLotesAsync();
}
