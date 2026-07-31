using Microsoft.Extensions.DependencyInjection;

namespace Osint.Persistence;

// Sin persistencia SQL por ahora (ver .claude/skill-decisions.md) — no se
// llama desde Program.cs. Queda scaffoldeado para retomar el punto 4 de
// _plan/plan-trabajo.md sin reestructurar la solución.
public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, string connectionString)
    {
        return services;
    }
}
