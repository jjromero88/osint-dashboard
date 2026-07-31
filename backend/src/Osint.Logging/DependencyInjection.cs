using Microsoft.Extensions.DependencyInjection;
using Osint.Common.Logging;

namespace Osint.Logging;

public static class DependencyInjection
{
    public static IServiceCollection AddAppLogging(this IServiceCollection services)
    {
        services.AddLogging();
        services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));
        return services;
    }
}
