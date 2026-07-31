namespace Osint.Application.DTOs;

public class SenalDto
{
    public string tipo { get; set; } = string.Empty;
    public string valor { get; set; } = string.Empty;
    public string source_url { get; set; } = string.Empty;
    public double confidence { get; set; }
    public DateTime captured_at { get; set; }
    public string herramienta { get; set; } = string.Empty;
}
