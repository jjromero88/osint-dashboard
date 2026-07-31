using Microsoft.Extensions.Logging;
using Osint.Common.Logging;

namespace Osint.Logging;

public class AppLogger<T> : IAppLogger<T>
{
    private readonly ILogger<T> _logger;

    public AppLogger(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object?[] args) => _logger.LogInformation(message, args);

    public void LogWarning(string message, params object?[] args) => _logger.LogWarning(message, args);

    public void LogError(Exception ex, string message, params object?[] args) => _logger.LogError(ex, message, args);
}
