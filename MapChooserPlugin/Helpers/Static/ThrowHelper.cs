namespace MapChooserPlugin.Helpers.Static;

public static class ThrowHelper
{
    private static string LogPrefix => MapChooserPlugin.LogPrefix;
    
    public static Exception GetConfigurationException(string configurationEntry, string error)
    {
        return new Exception($"{LogPrefix} Configuration Error with config entry \"{configurationEntry}\": {error}");
    }

    public static Exception GetConVarNotFoundException(string conVarName)
    {
        return new Exception($"{LogPrefix} Unable to find \"{conVarName}\" convar!");
    }
}