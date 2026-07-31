namespace Osint.Domain.Entities;

// Unidad atómica normalizada que devuelve cualquier herramienta OSINT.
public class Senal
{
    public string tipo { get; set; } = string.Empty;
    public string valor { get; set; } = string.Empty;
    public string source_url { get; set; } = string.Empty;
    public double confidence { get; set; }
    public DateTime captured_at { get; set; }
    public string herramienta { get; set; } = string.Empty;
}
