using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Osint.Application.Interfaces;
using Osint.Domain.Entities;

namespace Osint.Infrastructure.Clients;

// HTTP nativo — POST /api/v2/numbers {number} (sin '+' ni símbolos, ver
// _plan/plan-trabajo.md punto 2). No expone /health; se usa la raíz como probe.
public class PhoneInfogaClient : IOsintToolClient
{
    private readonly HttpClient _httpClient;

    public string Tipo => "phone";
    public string Herramienta => "phoneinfoga";

    public PhoneInfogaClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    // PhoneInfoga no tiene nada que modular por nivel — ver plan-trabajo.md §8.4.1.
    public async Task<ResultadoHerramienta> BuscarAsync(string objetivo, string nivel, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var numero = new string(objetivo.Where(char.IsDigit).ToArray());

        var response = await _httpClient.PostAsJsonAsync("/api/v2/numbers", new { number = numero }, cancellationToken);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return new ResultadoHerramienta { estado = "failed", raw = raw, duration_ms = (int)stopwatch.ElapsedMilliseconds };

        var info = System.Text.Json.JsonSerializer.Deserialize<PhoneInfogaResponse>(raw,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        var senales = new List<Senal>();
        if (info is { valid: true })
        {
            senales.Add(new Senal
            {
                tipo = "phone_info",
                valor = $"{info.country} ({info.countryCode}) — {info.local}, formato E.164: {info.e164}" +
                        (string.IsNullOrEmpty(info.carrier) ? "" : $", operador: {info.carrier}"),
                source_url = string.Empty,
                confidence = 1.0,
                captured_at = DateTime.UtcNow,
                herramienta = Herramienta
            });
        }

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
            var response = await _httpClient.GetAsync("/", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private class PhoneInfogaResponse
    {
        public bool valid { get; set; }
        [JsonPropertyName("e164")] public string? e164 { get; set; }
        public string? local { get; set; }
        public string? international { get; set; }
        public string? country { get; set; }
        public int countryCode { get; set; }
        public string? carrier { get; set; }
    }
}
