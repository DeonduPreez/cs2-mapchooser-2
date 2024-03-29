namespace MapChooserPlugin.Interfaces;

public interface IRandomizationHelper
{
    List<string> ExcludedMaps { get; }
    string GetRandomizedMap();
    List<string> GetShuffledAvailableMaps();
    void ExcludeMap(string mapName);
}