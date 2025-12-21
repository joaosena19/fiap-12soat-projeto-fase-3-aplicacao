using Application.Contracts.Monitoramento;
using Microsoft.Extensions.Logging;
using Serilog.Context;

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
        using (PushAllProperties())
        {
            _logger.LogInformation(message, args);
        }
    }

    public void LogWarning(string message, params object[] args)
    {
        using (PushAllProperties())
        {
            _logger.LogWarning(message, args);
        }
    }

    public void LogError(string message, params object[] args)
    {
        using (PushAllProperties())
        {
            _logger.LogError(message, args);
        }
    }

    public void LogError(Exception ex, string message, params object[] args)
    {
        using (PushAllProperties())
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

    private IDisposable PushAllProperties()
    {
        var disposables = new List<IDisposable>();
        
        foreach (var kvp in _context)
        {
            if (kvp.Value != null)
            {
                disposables.Add(LogContext.PushProperty(kvp.Key, kvp.Value));
            }
        }

        return new CompositeDisposable(disposables);
    }

    private class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables;
        private bool _disposed = false;

        public CompositeDisposable(List<IDisposable> disposables)
        {
            _disposables = disposables;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
                _disposables.Clear();
                _disposed = true;
            }
        }
    }
}