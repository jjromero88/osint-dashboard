using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Osint.Application.Common;
using Osint.Application.Interfaces;

namespace Osint.Validator;

public class ValidatorService : IValidatorService
{
    private readonly IServiceProvider _serviceProvider;

    public ValidatorService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<SpResult> ValidateAsync<T>(T instance)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();
        if (validator is null)
            return new SpResult();

        var result = await validator.ValidateAsync(instance!);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => e.ErrorMessage).ToList();
            return new SpResult { Error = true, Msg = errors.First(), Errors = errors };
        }
        return new SpResult();
    }
}
