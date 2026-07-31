using Microsoft.Extensions.DependencyInjection;
using Osint.Mapper.Profiles;

namespace Osint.Mapper;

public static class DependencyInjection
{
    public static IServiceCollection AddMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfile<BusquedaProfile>();
        });
        return services;
    }
}
