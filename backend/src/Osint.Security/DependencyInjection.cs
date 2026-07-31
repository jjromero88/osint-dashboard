using Microsoft.Extensions.DependencyInjection;

namespace Osint.Security;

// Sin uso por ahora: no hay modelo de usuario/login ni IDs a encriptar
// mientras no haya persistencia (ver .claude/skill-decisions.md).
public static class DependencyInjection
{
    public static IServiceCollection AddSecurity(this IServiceCollection services)
    {
        return services;
    }
}
