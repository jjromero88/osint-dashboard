using System.Diagnostics;
using System.Text.Json;
using Osint.Application.Interfaces;
using Osint.Domain.Entities;

namespace Osint.Infrastructure.Clients;

// HTTP nativo (API REST oficial, no wrapper — ver _plan/plan-trabajo.md §0).
// GET /query?source=...&domain=... — source acepta varias fuentes separadas
// por coma (confirmado en su propio OpenAPI).
//
// Niveles de profundidad (plan-trabajo.md §8.4.1): las fuentes de cada nivel
// se verificaron contra el api-keys.yaml real de theHarvester — solo se
// usan las que NO tienen ninguna entrada ahí (sin mecanismo de clave).
public class HarvesterClient : IOsintToolClient
{
    private static readonly Dictionary<string, (string[] Fuentes, int Limite)> ConfigPorNivel = new()
    {
        ["rapido"] = (["crtsh"], 200),
        ["medio"] = (["crtsh", "certspotter", "rapiddns", "subdomaincenter"], 500),
        ["profundo"] = (["crtsh", "certspotter", "rapiddns", "subdomaincenter", "urlscan", "sublist3r", "otx", "waybackarchive", "commoncrawl", "threatcrowd"], 1000)
    };

    private readonly HttpClient _httpClient;

    public string Tipo => "domain";
    public string Herramienta => "theharvester";

    public HarvesterClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResultadoHerramienta> BuscarAsync(string objetivo, string nivel, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var (fuentes, limite) = ConfigPorNivel.GetValueOrDefault(nivel, ConfigPorNivel["medio"]);
        // /query espera `source` repetido por cada fuente (array de query params),
        // no una lista separada por comas — confirmado probando contra la instancia
        // real: un solo "source=a,b,c" devuelve 400 "Source '...' is not supported".
        var fuentesQuery = string.Join("&", fuentes.Select(f => $"source={Uri.EscapeDataString(f)}"));
        var url = $"/query?{fuentesQuery}&domain={Uri.EscapeDataString(objetivo)}" +
                   $"&dns_brute=false&dns_lookup=false&proxies=false&shodan=false&take_over=false&api_scan=false&limit={limite}&start=0";

        var response = await _httpClient.GetAsync(url, cancellationToken);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return new ResultadoHerramienta { estado = "failed", raw = raw, duration_ms = (int)stopwatch.ElapsedMilliseconds };

        var query = JsonSerializer.Deserialize<HarvesterResponse>(raw,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new HarvesterResponse();

        var ahora = DateTime.UtcNow;
        var senales = new List<Senal>();

        senales.AddRange(query.hosts.Select(h => new Senal
        {
            tipo = "subdomain_found",
            valor = h,
            source_url = $"https://{h}",
            confidence = 0.7,
            captured_at = ahora,
            herramienta = Herramienta
        }));
        senales.AddRange(query.emails.Select(e => new Senal
        {
            tipo = "email_found",
            valor = e,
            source_url = string.Empty,
            confidence = 0.6,
            captured_at = ahora,
            herramienta = Herramienta
        }));
        senales.AddRange(query.ips.Select(ip => new Senal
        {
            tipo = "ip_found",
            valor = ip,
            source_url = string.Empty,
            confidence = 0.6,
            captured_at = ahora,
            herramienta = Herramienta
        }));
        senales.AddRange(query.interesting_urls.Select(u => new Senal
        {
            tipo = "interesting_url",
            valor = u,
            source_url = u,
            confidence = 0.6,
            captured_at = ahora,
            herramienta = Herramienta
        }));

        return new ResultadoHerramienta
        {
            estado = "ok",
            senales = senales,
            raw = raw,
            duration_ms = (int)stopwatch.ElapsedMilliseconds
        };
    }

    public async Task<bool> EstaSaludableAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync("/sources", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private class HarvesterResponse
    {
        public List<string> asns { get; set; } = [];
        public List<string> interesting_urls { get; set; } = [];
        public List<string> twitter_people { get; set; } = [];
        public List<Dictionary<string, object>> linkedin_people { get; set; } = [];
        public List<string> linkedin_links { get; set; } = [];
        public List<string> trello_urls { get; set; } = [];
        public List<string> ips { get; set; } = [];
        public List<string> emails { get; set; } = [];
        public List<string> hosts { get; set; } = [];
    }
}
