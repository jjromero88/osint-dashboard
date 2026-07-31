namespace Osint.Infrastructure.Clients;

// Contrato normalizado que comparten los wrappers propios (Holehe, Maigret) —
// ver osint-dashboard-endpoints.md §2 y wrappers/*/app/schemas/scan.py.
internal class WrapperScanResponse
{
    public string tool { get; set; } = string.Empty;
    public string target { get; set; } = string.Empty;
    public string status { get; set; } = string.Empty;
    public List<WrapperSignal> signals { get; set; } = [];
    public string raw { get; set; } = string.Empty;
    public int duration_ms { get; set; }
}

internal class WrapperSignal
{
    public string type { get; set; } = string.Empty;
    public string value { get; set; } = string.Empty;
    public string source_url { get; set; } = string.Empty;
    public double confidence { get; set; }
    public DateTime captured_at { get; set; }
}
