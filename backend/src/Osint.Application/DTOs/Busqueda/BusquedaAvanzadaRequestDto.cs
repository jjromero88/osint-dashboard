using Osint.Application.Common;

namespace Osint.Application.DTOs;

// Body de POST /api/search/advanced — modo avanzado (multi-dato de la misma persona).
public class BusquedaAvanzadaRequestDto
{
    public List<string> usernames { get; set; } = [];
    public List<string> emails { get; set; } = [];
    public List<string> phones { get; set; } = [];
    public List<string> domains { get; set; } = [];
    public List<string> names { get; set; } = [];
    public string nivel { get; set; } = CatalogoNiveles.Default;
}
