namespace Osint.Application.DTOs;

// Un hallazgo único del modo avanzado, deduplicado por source_url.
public class HallazgoDto
{
    public string tipo { get; set; } = string.Empty;
    public string valor { get; set; } = string.Empty;
    public string source_url { get; set; } = string.Empty;
    public double confidence { get; set; }
    public List<EncontradoViaDto> encontrado_via { get; set; } = [];
}
