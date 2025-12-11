using Application.Contracts.Monitoramento;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Monitoramento;

public class LoggerAdapter<T> : IAppLogger
{
    private readonly ILogger<T> _logger;

    public LoggerAdapter(ILogger<T> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message, params object[] args)
    {
        _logger.LogInformation(message, args);
    }

    public void LogWarning(string message, params object[] args)
    {
        _logger.LogWarning(message, args);
    }

    public void LogError(string message, params object[] args)
    {
        _logger.LogError(message, args);
    }

    public void LogError(Exception ex, string message, params object[] args)
    {
        _logger.LogError(ex, message, args);
    }

    public IAppLogger ComPropriedade(string key, object? value)
    {
        var context = new Dictionary<string, object?> { [key] = value };
        return new ContextualLogger(_logger, context);
    }
}