namespace Osint.Application.DTOs;

// Catálogo GET /api/tools — qué herramienta cubre cada tipo de búsqueda, con
// etiqueta legible para alimentar el <select> del front (value=tipo, label=descripcion).
public class HerramientaDto
{
    public string tipo { get; set; } = string.Empty;
    public string descripcion { get; set; } = string.Empty;
    public string herramienta { get; set; } = string.Empty;
}
