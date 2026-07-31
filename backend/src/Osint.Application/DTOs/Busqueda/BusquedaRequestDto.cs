namespace Osint.Application.DTOs;

// Body de POST /api/search — modo básico (un dato, un tipo).
public class BusquedaRequestDto
{
    public string tipo { get; set; } = string.Empty;
    public string objetivo { get; set; } = string.Empty;
}
