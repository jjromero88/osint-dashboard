namespace Osint.Domain.Entities;

// Lo que devuelve un cliente de herramienta (Infrastructure) tras ejecutar la búsqueda.
public class ResultadoHerramienta
{
    public string estado { get; set; } = "ok";
    public List<Senal> senales { get; set; } = [];
    public string raw { get; set; } = string.Empty;
    public int duration_ms { get; set; }
}
