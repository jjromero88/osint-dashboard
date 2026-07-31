namespace Osint.Application.Common;

public record NivelBusqueda(string Value, string Desc);

// Única fuente de verdad de los niveles de profundidad válidos (sin BD por
// ahora, mismo criterio que CatalogoTipos). La usan el validador (whitelist)
// y los clientes de herramienta para decidir cuánto escarbar.
public static class CatalogoNiveles
{
    public const string Default = "medio";

    public static readonly IReadOnlyList<NivelBusqueda> Niveles =
    [
        new("rapido", "Rápido"),
        new("medio", "Medio"),
        new("profundo", "Profundo")
    ];

    public static bool EsValido(string value) => Niveles.Any(n => n.Value == value);
}
