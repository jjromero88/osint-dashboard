using FluentValidation;
using Osint.Application.Common;
using Osint.Application.DTOs;

namespace Osint.Validator.Validators;

public class BusquedaRequestValidator : AbstractValidator<BusquedaRequestDto>
{
    public BusquedaRequestValidator()
    {
        RuleFor(x => x.tipo)
            .NotEmpty().WithMessage("El campo tipo es obligatorio.")
            .Must(CatalogoTipos.EsValido)
            .WithMessage($"El campo tipo debe ser uno de: {string.Join(", ", CatalogoTipos.Tipos.Select(t => t.Value))}.");

        RuleFor(x => x.objetivo)
            .NotEmpty().WithMessage("El campo objetivo es obligatorio.")
            .MaximumLength(300).WithMessage("El campo objetivo no debe exceder 300 caracteres.");

        RuleFor(x => x.nivel)
            .Must(CatalogoNiveles.EsValido)
            .WithMessage($"El campo nivel debe ser uno de: {string.Join(", ", CatalogoNiveles.Niveles.Select(n => n.Value))}.");
    }
}
