using Microsoft.AspNetCore.Mvc;
using Osint.Application.Interfaces;

namespace Osint.WebApi.Controllers;

[ApiController]
[Route("api/tools")]
public class ToolsController : ControllerBase
{
    private readonly IHerramientasService _herramientasService;

    public ToolsController(IHerramientasService herramientasService)
    {
        _herramientasService = herramientasService;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var result = await _herramientasService.ListarAsync();
        return !result.Success ? BadRequest(result) : Ok(result);
    }

    [HttpGet("health")]
    public async Task<IActionResult> Salud()
    {
        var result = await _herramientasService.ObtenerSaludAsync();
        return !result.Success ? BadRequest(result) : Ok(result);
    }
}
