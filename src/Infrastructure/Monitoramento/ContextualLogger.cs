using Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Monitoramento;

public class ContextualLogger : IAppLogger
{
    private readonly ILogger _logger;
    private readonly Dictionary<string, object?> _context;

    public ContextualLogger(ILogger logger, Dictionary<string, object?> context)
    {
        _logger = logger;
        _context = context;
    }

    public void LogInformation(string message, params object[] args)
    {
        using (_logger.BeginScope(_context))
        {
            _logger.LogInformation(message, args);
        }
    }

    public void LogWarning(string message, params object[] args)
    {
        using (_logger.BeginScope(_context))
        {
            _logger.LogWarning(message, args);
        }
    }

    public void LogError(string message, params object[] args)
    {
        using (_logger.BeginScope(_context))
        {
            _logger.LogError(message, args);
        }
    }

    public void LogError(Exception ex, string message, params object[] args)
    {
        using (_logger.BeginScope(_context))
        {
            _logger.LogError(ex, message, args);
        }
    }

    public IAppLogger ComPropriedade(string key, object? value)
    {
        var newContext = new Dictionary<string, object?>(_context)
        {
            [key] = value
        };
        return new ContextualLogger(_logger, newContext);
    }
}