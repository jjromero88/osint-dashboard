using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Osint.Application.Interfaces;

namespace Osint.Validator;

public static class DependencyInjection
{
    public static IServiceCollection AddValidator(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<IValidatorService, ValidatorService>();
        return services;
    }
}
