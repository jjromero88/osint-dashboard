using Microsoft.AspNetCore.Mvc;
using Osint.Application.DTOs;
using Osint.Application.Interfaces;

namespace Osint.WebApi.Controllers;

// Sin [Authorize] por ahora — no hay modelo de usuario/login todavía
// (ver .claude/skill-decisions.md).
[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly IBusquedaService _busquedaService;
    private readonly IBusquedaAvanzadaService _busquedaAvanzadaService;

    public SearchController(IBusquedaService busquedaService, IBusquedaAvanzadaService busquedaAvanzadaService)
    {
        _busquedaService = busquedaService;
        _busquedaAvanzadaService = busquedaAvanzadaService;
    }

    // Modo básico: un dato, un tipo (phone|email|username|domain|aggregate)
    [HttpPost]
    public async Task<IActionResult> Iniciar([FromBody] BusquedaRequestDto dto)
    {
        var result = await _busquedaService.IniciarBusquedaAsync(dto);
        return !result.Success ? BadRequest(result) : Accepted(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(Guid id)
    {
        var result = await _busquedaService.ObtenerBusquedaAsync(id);
        return !result.Success ? BadRequest(result) : Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var result = await _busquedaService.ListarBusquedasAsync();
        return !result.Success ? BadRequest(result) : Ok(result);
    }

    // Modo avanzado: varios datos de la misma persona a la vez, consolidados
    [HttpPost("advanced")]
    public async Task<IActionResult> IniciarAvanzada([FromBody] BusquedaAvanzadaRequestDto dto)
    {
        var result = await _busquedaAvanzadaService.IniciarBusquedaAvanzadaAsync(dto);
        return !result.Success ? BadRequest(result) : Accepted(result);
    }

    [HttpGet("advanced/{batchId}")]
    public async Task<IActionResult> ObtenerAvanzada(Guid batchId)
    {
        var result = await _busquedaAvanzadaService.ObtenerBusquedaAvanzadaAsync(batchId);
        return !result.Success ? BadRequest(result) : Ok(result);
    }

    [HttpGet("advanced")]
    public async Task<IActionResult> ListarAvanzada()
    {
        var result = await _busquedaAvanzadaService.ListarLotesAsync();
        return !result.Success ? BadRequest(result) : Ok(result);
    }
}
