namespace Osint.Infrastructure.Options;

// URLs base de las 5 herramientas/wrappers ya probados (ver _plan/plan-trabajo.md).
// Localhost porque el backend corre fuera de docker-compose por ahora; si se
// dockeriza, cambiar por los nombres de servicio de la red osint-net.
public class ToolsOptions
{
    public const string SeccionConfig = "Tools";

    public string PhoneInfoga { get; set; } = "http://localhost:5010";
    public string SpiderFoot { get; set; } = "http://localhost:5001";
    public string Harvester { get; set; } = "http://localhost:8001";
    public string Maigret { get; set; } = "http://localhost:8002";
    public string Holehe { get; set; } = "http://localhost:8003";
}
