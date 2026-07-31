namespace Osint.Application.DTOs;

public class ResumenDto
{
    public int total_unico { get; set; }
    public Dictionary<string, int> por_tipo { get; set; } = [];
}
