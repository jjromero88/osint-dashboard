using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Osint.Application.Interfaces;
using Osint.Infrastructure.Clients;
using Osint.Infrastructure.Options;
using Osint.Infrastructure.Queue;
using Osint.Infrastructure.Store;

namespace Osint.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var tools = configuration.GetSection(ToolsOptions.SeccionConfig).Get<ToolsOptions>() ?? new ToolsOptions();

        // AddHttpClient<TClient, TImplementation> con el mismo TClient (IOsintToolClient) para
        // los 5 pisaría la configuración entre sí: el nombre interno del cliente HTTP se deriva
        // de TClient, no de TImplementation, así que la última llamada gana para todos. Se
        // registra cada implementación por su propio tipo, y luego se expone como
        // IOsintToolClient vía factory — así cada una tiene su HttpClient configurado aparte.
        services.AddHttpClient<PhoneInfogaClient>(c => c.BaseAddress = new Uri(tools.PhoneInfoga));
        services.AddHttpClient<HoleheClient>(c => c.BaseAddress = new Uri(tools.Holehe));
        services.AddHttpClient<MaigretClient>(c => c.BaseAddress = new Uri(tools.Maigret));
        // Timeout default de HttpClient (100s) se queda corto en nivel "profundo"
        // (varias fuentes externas encadenadas dentro del propio /query de
        // theHarvester) — probado en vivo, ver plan-trabajo.md §8.4.1.
        services.AddHttpClient<HarvesterClient>(c =>
        {
            c.BaseAddress = new Uri(tools.Harvester);
            c.Timeout = TimeSpan.FromMinutes(10);
        });
        // SpiderFoot: /startscan responde 303 — hay que leer el header Location a mano, sin seguir el redirect.
        services.AddHttpClient<SpiderFootClient>(c => c.BaseAddress = new Uri(tools.SpiderFoot))
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });

        services.AddTransient<IOsintToolClient>(sp => sp.GetRequiredService<PhoneInfogaClient>());
        services.AddTransient<IOsintToolClient>(sp => sp.GetRequiredService<HoleheClient>());
        services.AddTransient<IOsintToolClient>(sp => sp.GetRequiredService<MaigretClient>());
        services.AddTransient<IOsintToolClient>(sp => sp.GetRequiredService<HarvesterClient>());
        services.AddTransient<IOsintToolClient>(sp => sp.GetRequiredService<SpiderFootClient>());

        services.AddSingleton<IBusquedaStore, BusquedaMemoryStore>();
        services.AddSingleton<IBusquedaQueue, BusquedaChannelQueue>();
        services.AddHostedService<BusquedaWorker>();

        return services;
    }
}
