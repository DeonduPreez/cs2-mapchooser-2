namespace MapChooserPlugin.Interfaces;

public interface ILogHelper
{
    void LogTrace(string message);
    void LogDebug(string message);
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message);
    void LogCritical(string message);
    void LogConfigurationWarning(string configurationEntry, string warning);
}