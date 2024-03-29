using MapChooserPlugin.Interfaces;
using MapChooserPlugin.Models.Config;
using Microsoft.Extensions.Logging;

namespace MapChooserPlugin.Helpers;

public class LogHelper : ILogHelper
{
    private readonly MapChooserConfig _config;
    private static string LogPrefix => MapChooserPlugin.LogPrefix;

    public LogHelper(MapChooserConfig config)
    {
        _config = config;
    }

    private void Log(string message, LogLevel level)
    {
        var shouldLog = CheckLogLevel(level);
        if (!shouldLog)
        {
            return;
        }

        Console.ResetColor();
        switch (level)
        {
            case LogLevel.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogLevel.Error:
            case LogLevel.Critical:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            default:
                Console.ForegroundColor = ConsoleColor.White;
                break;
        }

        Console.WriteLine($"{LogPrefix} {message}");
        Console.ResetColor();
    }

    public void LogTrace(string message)
    {
        Log(message, LogLevel.Trace);
    }

    public void LogDebug(string message)
    {
        Log(message, LogLevel.Debug);
    }

    public void LogInformation(string message)
    {
        Log(message, LogLevel.Information);
    }

    public void LogWarning(string message)
    {
        Log(message, LogLevel.Warning);
    }

    public void LogError(string message)
    {
        Log(message, LogLevel.Error);
    }

    public void LogCritical(string message)
    {
        Log(message, LogLevel.Critical);
    }

    public void LogConfigurationWarning(string configurationEntry, string warning)
    {
        LogWarning($"Configuration Warning with config entry \"{configurationEntry}\": {warning}");
    }

    private bool CheckLogLevel(LogLevel level)
    {
        if (_config.LogLevel == LogLevel.None)
        {
            return false;
        }

        return level >= _config.LogLevel;
    }
}