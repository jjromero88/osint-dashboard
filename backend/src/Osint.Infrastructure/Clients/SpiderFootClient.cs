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
//
// Niveles de profundidad (plan-trabajo.md §8.4.1): en vez de dejar que
// SpiderFoot elija módulos por `usecase=Footprint`, se manda un `modulelist`
// explícito según el nivel. Los 3 conjuntos de abajo se verificaron contra
// GET /modules + GET /optsraw de la instancia real: de 230 módulos, 148 no
// exponen ninguna opción *_api_key/api_key — son los únicos candidatos.
// Cada nivel incluye al anterior (Medio = Rápido + extra; Profundo = todos
// los 148 sin clave).
public class SpiderFootClient : IOsintToolClient
{
    // Núcleo pasivo: resolución DNS, WHOIS, certificados TLS.
    private static readonly string[] ModulosRapido =
    [
        "sfp_dnsresolve", "sfp_dnsraw", "sfp_whois", "sfp_ripe", "sfp_arin",
        "sfp_sslcert", "sfp_crt", "sfp_hackertarget", "sfp_dnsdumpster", "sfp_reversewhois"
    ];

    // Se suma a Rápido: subdominios por fuentes libres, presencia en redes
    // públicas, correlaciones básicas (nombre/empresa/país).
    private static readonly string[] ModulosMedioExtra =
    [
        "sfp_sublist3r", "sfp_crobat_api", "sfp_dnsneighbor", "sfp_social", "sfp_accounts",
        "sfp_email", "sfp_names", "sfp_company", "sfp_similar", "sfp_archiveorg",
        "sfp_urlscan", "sfp_commoncrawl", "sfp_webframework", "sfp_webserver", "sfp_pageinfo",
        "sfp_cookie", "sfp_spider", "sfp_threatcrowd", "sfp_tldsearch", "sfp_countryname"
    ];

    // Todo lo gratuito — los 148 módulos verificados sin ninguna opción de API key.
    private static readonly string[] ModulosProfundo =
    [
        "sfp_abusech", "sfp_accounts", "sfp_adblock", "sfp_adguard_dns", "sfp_ahmia",
        "sfp_alienvaultiprep", "sfp_apple_itunes", "sfp_archiveorg", "sfp_arin", "sfp_azureblobstorage",
        "sfp_base64", "sfp_bgpview", "sfp_binstring", "sfp_bitcoin", "sfp_blockchain",
        "sfp_blocklistde", "sfp_botvrij", "sfp_callername", "sfp_cinsscore", "sfp_cleanbrowsing",
        "sfp_cleantalk", "sfp_cloudflaredns", "sfp_coinblocker", "sfp_commoncrawl", "sfp_comodo",
        "sfp_company", "sfp_cookie", "sfp_countryname", "sfp_creditcard", "sfp_crobat_api",
        "sfp_crossref", "sfp_crt", "sfp_crxcavator", "sfp_customfeed", "sfp_cybercrimetracker",
        "sfp_debounce", "sfp_digitaloceanspace", "sfp_dns_for_family", "sfp_dnsbrute", "sfp_dnscommonsrv",
        "sfp_dnsdumpster", "sfp_dnsgrep", "sfp_dnsneighbor", "sfp_dnsraw", "sfp_dnsresolve",
        "sfp_dnszonexfer", "sfp_dronebl", "sfp_duckduckgo", "sfp_email", "sfp_emailformat",
        "sfp_emergingthreats", "sfp_errors", "sfp_ethereum", "sfp_filemeta", "sfp_flickr",
        "sfp_fortinet", "sfp_fsecure_riddler", "sfp_github", "sfp_gleif", "sfp_google_tag_manager",
        "sfp_googleobjectstorage", "sfp_gravatar", "sfp_greensnow", "sfp_grep_app", "sfp_h1nobbdde",
        "sfp_hackertarget", "sfp_hashes", "sfp_hosting", "sfp_iban", "sfp_intfiles",
        "sfp_ipapico", "sfp_isc", "sfp_junkfiles", "sfp_keybase", "sfp_maltiverse",
        "sfp_mnemonic", "sfp_multiproxy", "sfp_myspace", "sfp_names", "sfp_onionsearchengine",
        "sfp_openbugbounty", "sfp_opendns", "sfp_opennic", "sfp_openphish", "sfp_openstreetmap",
        "sfp_pageinfo", "sfp_pgp", "sfp_phishstats", "sfp_phishtank", "sfp_phone",
        "sfp_portscan_tcp", "sfp_psbdmp", "sfp_punkspider", "sfp_quad9", "sfp_reversewhois",
        "sfp_ripe", "sfp_robtex", "sfp_s3bucket", "sfp_searchcode", "sfp_similar",
        "sfp_skymem", "sfp_slideshare", "sfp_social", "sfp_sorbs", "sfp_spamcop",
        "sfp_spamhaus", "sfp_spider", "sfp_sslcert", "sfp_stevenblack_hosts", "sfp_strangeheaders",
        "sfp_subdomain_takeover", "sfp_sublist3r", "sfp_surbl", "sfp_talosintel", "sfp_threatcrowd",
        "sfp_threatfox", "sfp_threatminer", "sfp_tldsearch", "sfp_tool_cmseek", "sfp_tool_dnstwist",
        "sfp_tool_nbtscan", "sfp_tool_nmap", "sfp_tool_nuclei", "sfp_tool_onesixtyone", "sfp_tool_retirejs",
        "sfp_tool_snallygaster", "sfp_tool_testsslsh", "sfp_tool_trufflehog", "sfp_tool_wafw00f", "sfp_tool_wappalyzer",
        "sfp_tool_whatweb", "sfp_torch", "sfp_torexits", "sfp_trumail", "sfp_twitter",
        "sfp_uceprotect", "sfp_urlscan", "sfp_venmo", "sfp_voipbl", "sfp_vxvault",
        "sfp_webanalytics", "sfp_webframework", "sfp_webserver", "sfp_whois", "sfp_wikileaks",
        "sfp_wikipediaedits", "sfp_yandexdns", "sfp_zoneh"
    ];

    private static readonly Dictionary<string, string[]> ModulosPorNivel = new()
    {
        ["rapido"] = ModulosRapido,
        ["medio"] = [.. ModulosRapido, .. ModulosMedioExtra],
        ["profundo"] = ModulosProfundo
    };

    // Rápido: sin cambio respecto al timeout de antes de introducir niveles.
    // Medio/Profundo: rangos sugeridos en osint-dashboard-v0.1.md §3.2.
    private static readonly Dictionary<string, TimeSpan> TiempoMaximoEsperaPorNivel = new()
    {
        ["rapido"] = TimeSpan.FromSeconds(90),
        ["medio"] = TimeSpan.FromMinutes(5),
        ["profundo"] = TimeSpan.FromMinutes(18)
    };

    private static readonly TimeSpan IntervaloPolling = TimeSpan.FromSeconds(3);

    private readonly HttpClient _httpClient;

    public string Tipo => "aggregate";
    public string Herramienta => "spiderfoot";

    public SpiderFootClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResultadoHerramienta> BuscarAsync(string objetivo, string nivel, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var modulos = ModulosPorNivel.GetValueOrDefault(nivel, ModulosPorNivel["medio"]);
        var tiempoMaximo = TiempoMaximoEsperaPorNivel.GetValueOrDefault(nivel, TiempoMaximoEsperaPorNivel["medio"]);

        var scanId = await IniciarEscaneoAsync(objetivo, modulos, cancellationToken);
        if (scanId is null)
            return new ResultadoHerramienta { estado = "failed", raw = "No se pudo iniciar el escaneo en SpiderFoot.", duration_ms = (int)stopwatch.ElapsedMilliseconds };

        var (estado, rawStatus) = await EsperarFinalizacionAsync(scanId, tiempoMaximo, cancellationToken);
        var senales = await ObtenerHallazgosAsync(scanId, cancellationToken);

        return new ResultadoHerramienta
        {
            estado = estado,
            senales = senales,
            raw = rawStatus,
            duration_ms = (int)stopwatch.ElapsedMilliseconds
        };
    }

    // POST /startscan (form-data) — SpiderFoot responde 303 con el id en Location.
    // modulelist reemplaza a usecase=Footprint: el nivel decide los módulos exactos.
    private async Task<string?> IniciarEscaneoAsync(string objetivo, string[] modulos, CancellationToken cancellationToken)
    {
        var body = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["scanname"] = $"busqueda-{DateTime.UtcNow:yyyyMMddHHmmss}",
            ["scantarget"] = FormatearObjetivo(objetivo),
            ["usecase"] = "",
            ["modulelist"] = string.Join(",", modulos),
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

    private async Task<(string estado, string raw)> EsperarFinalizacionAsync(string scanId, TimeSpan tiempoMaximo, CancellationToken cancellationToken)
    {
        var inicio = DateTime.UtcNow;
        string ultimoRaw = string.Empty;

        while (DateTime.UtcNow - inicio < tiempoMaximo)
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
