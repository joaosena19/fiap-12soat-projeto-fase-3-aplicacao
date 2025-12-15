namespace Application.Contracts.Monitoramento;

public interface IAppLogger
{
    void LogInformation(string message, params object[] args);
    void LogWarning(string message, params object[] args);
    void LogError(string message, params object[] args);
    void LogError(Exception ex, string message, params object[] args);
    IAppLogger ComPropriedade(string key, object? value);
}