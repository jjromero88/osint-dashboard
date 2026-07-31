using System.Diagnostics;
using System.Net;
using System.Text.Json;
using Osint.Application.Interfaces;
using Osint.Domain.Entities;

namespace Osint.Infrastructure.Clients;

// HTTP nativo — POST /startscan (form-data, no JSON) devuelve 303 con el id
// en el header Location; hay que hacer polling a /scanstatus hasta que
// termine y después leer /scaneventresults. Es la búsqueda más lenta de las
// 5 — timeout propio más largo (ver _plan/plan-trabajo.md punto 2).
public class SpiderFootClient : IOsintToolClient
{
    private static readonly TimeSpan TiempoMaximoEspera = TimeSpan.FromSeconds(90);
    private static readonly TimeSpan IntervaloPolling = TimeSpan.FromSeconds(3);

    private readonly HttpClient _httpClient;

    public string Tipo => "aggregate";
    public string Herramienta => "spiderfoot";

    public SpiderFootClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResultadoHerramienta> BuscarAsync(string objetivo, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        var scanId = await IniciarEscaneoAsync(objetivo, cancellationToken);
        if (scanId is null)
            return new ResultadoHerramienta { estado = "failed", raw = "No se pudo iniciar el escaneo en SpiderFoot.", duration_ms = (int)stopwatch.ElapsedMilliseconds };

        var (estado, rawStatus) = await EsperarFinalizacionAsync(scanId, cancellationToken);
        var senales = await ObtenerHallazgosAsync(scanId, cancellationToken);

        return new ResultadoHerramienta
        {
            estado = estado,
            senales = senales,
            raw = rawStatus,
            duration_ms = (int)stopwatch.ElapsedMilliseconds
        };
    }

    // POST /startscan (form-data) — SpiderFoot responde 303 con el id en Location
    private async Task<string?> IniciarEscaneoAsync(string objetivo, CancellationToken cancellationToken)
    {
        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["scanname"] = $"busqueda-{DateTime.UtcNow:yyyyMMddHHmmss}",
            ["scantarget"] = FormatearObjetivo(objetivo),
            ["usecase"] = "Footprint",
            ["modulelist"] = "",
            ["typelist"] = ""
        });

        var response = await _httpClient.PostAsync("/startscan", body, cancellationToken);
        var location = response.Headers.Location;
        if (location is null)
            return null;

        var idParam = location.Query.TrimStart('?')
            .Split('&')
            .FirstOrDefault(p => p.StartsWith("id=", StringComparison.OrdinalIgnoreCase));
        return idParam?["id=".Length..];
    }

    // SpiderFoot detecta el tipo de target por el formato del string: dominios, emails,
    // teléfonos (+E.164) e IPs se mandan tal cual; username/nombre de persona los rechaza
    // si no vienen entre comillas (ver ayuda de "New Scan" en su propia UI).
    private static string FormatearObjetivo(string objetivo)
    {
        if (objetivo.StartsWith('"'))
            return objetivo;
        if (objetivo.Contains('@'))
            return objetivo;
        if (objetivo.StartsWith('+') && objetivo[1..].All(char.IsDigit))
            return objetivo;
        if (System.Net.IPAddress.TryParse(objetivo, out _))
            return objetivo;
        if (objetivo.Contains('.') && !objetivo.Contains(' '))
            return objetivo;

        return $"\"{objetivo}\"";
    }

    private async Task<(string estado, string raw)> EsperarFinalizacionAsync(string scanId, CancellationToken cancellationToken)
    {
        var inicio = DateTime.UtcNow;
        string ultimoRaw = string.Empty;

        while (DateTime.UtcNow - inicio < TiempoMaximoEspera)
        {
            await Task.Delay(IntervaloPolling, cancellationToken);

            var response = await _httpClient.GetAsync($"/scanstatus?id={scanId}", cancellationToken);
            ultimoRaw = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
                continue;

            var campos = JsonSerializer.Deserialize<JsonElement[]>(ultimoRaw);
            var status = campos is { Length: > 5 } ? campos[5].GetString() : null;

            if (status is "FINISHED" or "ABORTED" or "ERROR-FAILED")
                return ("ok", ultimoRaw);
        }

        // Se acabó nuestro presupuesto de tiempo: pedimos que se detenga y devolvemos lo que haya
        await _httpClient.GetAsync($"/stopscan?id={scanId}", cancellationToken);
        return ("timeout", ultimoRaw);
    }

    // GET /scaneventresults?id=X&eventType=ALL — filas [timestamp, data, source_data, module, confidence, visibility, risk, hash, _, _, eventType]
    private async Task<List<Senal>> ObtenerHallazgosAsync(string scanId, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync($"/scaneventresults?id={scanId}&eventType=ALL", cancellationToken);
        if (!response.IsSuccessStatusCode)
            return [];

        var raw = await response.Content.ReadAsStringAsync(cancellationToken);
        var filas = JsonSerializer.Deserialize<JsonElement[][]>(raw) ?? [];

        var senales = new List<Senal>();
        foreach (var fila in filas)
        {
            if (fila.Length < 11)
                continue;

            var eventType = fila[10].GetString() ?? string.Empty;
            if (eventType is "ROOT" or "RAW_RIR_DATA")
                continue;

            var dato = WebUtility.HtmlDecode(fila[1].GetString() ?? string.Empty);
            var timestamp = fila[0].GetString();
            var confidencePct = fila[4].ValueKind == JsonValueKind.Number ? fila[4].GetDouble() : 100;

            senales.Add(new Senal
            {
                tipo = eventType.ToLowerInvariant(),
                valor = dato,
                source_url = dato.StartsWith("http://") || dato.StartsWith("https://") ? dato : string.Empty,
                confidence = confidencePct / 100.0,
                captured_at = DateTime.TryParse(timestamp, out var dt) ? DateTime.SpecifyKind(dt, DateTimeKind.Utc) : DateTime.UtcNow,
                herramienta = Herramienta
            });
        }

        return senales;
    }

    public async Task<bool> EstaSaludableAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync("/ping", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
