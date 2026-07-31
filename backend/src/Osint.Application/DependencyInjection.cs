using Microsoft.Extensions.DependencyInjection;
using Osint.Application.Interfaces;
using Osint.Application.Services;

namespace Osint.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IBusquedaService, BusquedaService>();
        services.AddScoped<IBusquedaAvanzadaService, BusquedaAvanzadaService>();
        services.AddScoped<IHerramientasService, HerramientasService>();
        return services;
    }
}
