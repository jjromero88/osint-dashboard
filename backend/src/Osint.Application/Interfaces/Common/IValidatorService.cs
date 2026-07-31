using Osint.Application.Common;

namespace Osint.Application.Interfaces;

public interface IValidatorService
{
    Task<SpResult> ValidateAsync<T>(T instance);
}
