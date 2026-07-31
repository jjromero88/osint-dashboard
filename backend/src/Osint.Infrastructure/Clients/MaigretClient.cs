using System.Diagnostics;
using System.Net.Http.Json;
using Osint.Application.Interfaces;
using Osint.Domain.Entities;

namespace Osint.Infrastructure.Clients;

// Wrapper propio — POST /scan {target: username} -> contrato normalizado ya
// probado en _plan/plan-trabajo.md punto 3. El wrapper ya corre con
// recursión/extracción desactivadas (--no-recursion --no-extracting).
public class MaigretClient : IOsintToolClient
{
    private readonly HttpClient _httpClient;

    public string Tipo => "username";
    public string Herramienta => "maigret";

    public MaigretClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResultadoHerramienta> BuscarAsync(string objetivo, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var response = await _httpClient.PostAsJsonAsync("/scan", new { target = objetivo }, cancellationToken);
        var raw = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            return new ResultadoHerramienta { estado = "failed", raw = raw, duration_ms = (int)stopwatch.ElapsedMilliseconds };

        var scan = System.Text.Json.JsonSerializer.Deserialize<WrapperScanResponse>(raw,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;

        return new ResultadoHerramienta
        {
            estado = scan.status,
            senales = scan.signals.Select(s => new Senal
            {
                tipo = s.type,
                valor = s.value,
                source_url = s.source_url,
                confidence = s.confidence,
                captured_at = s.captured_at,
                herramienta = Herramienta
            }).ToList(),
            raw = scan.raw,
            duration_ms = scan.duration_ms
        };
    }

    public async Task<bool> EstaSaludableAsync(CancellationToken cancellationToken)
    {
        try
        {
            var response = await _httpClient.GetAsync("/health", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}
