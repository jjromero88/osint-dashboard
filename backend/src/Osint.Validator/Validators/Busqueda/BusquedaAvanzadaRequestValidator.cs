using FluentValidation;
using Osint.Application.Common;
using Osint.Application.DTOs;

namespace Osint.Validator.Validators;

public class BusquedaAvanzadaRequestValidator : AbstractValidator<BusquedaAvanzadaRequestDto>
{
    private const int MaxPorCampo = 5;

    public BusquedaAvanzadaRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => x.usernames.Count + x.emails.Count + x.phones.Count + x.domains.Count + x.names.Count > 0)
            .WithMessage("Debe ingresar al menos un dato (username, email, teléfono, dominio o nombre) para buscar.");

        RuleFor(x => x.nivel)
            .Must(CatalogoNiveles.EsValido)
            .WithMessage($"El campo nivel debe ser uno de: {string.Join(", ", CatalogoNiveles.Niveles.Select(n => n.Value))}.");

        RuleFor(x => x.usernames).Must(l => l.Count <= MaxPorCampo)
            .WithMessage($"Máximo {MaxPorCampo} usernames por búsqueda.");
        RuleFor(x => x.emails).Must(l => l.Count <= MaxPorCampo)
            .WithMessage($"Máximo {MaxPorCampo} emails por búsqueda.");
        RuleFor(x => x.phones).Must(l => l.Count <= MaxPorCampo)
            .WithMessage($"Máximo {MaxPorCampo} teléfonos por búsqueda.");
        RuleFor(x => x.domains).Must(l => l.Count <= MaxPorCampo)
            .WithMessage($"Máximo {MaxPorCampo} dominios por búsqueda.");
        RuleFor(x => x.names).Must(l => l.Count <= MaxPorCampo)
            .WithMessage($"Máximo {MaxPorCampo} nombres por búsqueda.");
    }
}
