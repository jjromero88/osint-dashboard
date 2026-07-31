using Osint.Domain.Entities;

namespace Osint.Application.Interfaces;

// Puerto que implementa cada herramienta OSINT (uno por tool) en Infrastructure.
// Application/Domain no conocen HTTP ni la forma de respuesta de cada herramienta.
public interface IOsintToolClient
{
    // Tipo de búsqueda que cubre: phone|email|username|domain|aggregate
    string Tipo { get; }

    // Nombre de la herramienta detrás (phoneinfoga|holehe|maigret|theharvester|spiderfoot)
    string Herramienta { get; }

    Task<ResultadoHerramienta> BuscarAsync(string objetivo, CancellationToken cancellationToken);

    // Cada herramienta define su propio probe (ruta y forma varían — ver notas del punto 2 de la bitácora)
    Task<bool> EstaSaludableAsync(CancellationToken cancellationToken);
}
