namespace Osint.Application.Common;

public record TipoBusqueda(string Value, string Desc);

// Única fuente de verdad de los tipos de búsqueda válidos (sin BD por ahora —
// ver .claude/skill-decisions.md). La usan el validador (whitelist) y el
// catálogo GET /api/tools (para alimentar el <select> del front).
public static class CatalogoTipos
{
    public static readonly IReadOnlyList<TipoBusqueda> Tipos =
    [
        new("phone", "Teléfono"),
        new("email", "Email"),
        new("username", "Nombre de usuario"),
        new("domain", "Dominio"),
        new("aggregate", "Búsqueda agregada (perfil profundo)")
    ];

    public static bool EsValido(string value) => Tipos.Any(t => t.Value == value);
}
