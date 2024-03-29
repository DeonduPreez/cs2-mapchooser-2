using System.Text.Json;
using ListShuffle;
using MapChooserPlugin.Classes.Abstract;
using MapChooserPlugin.Interfaces;
using MapChooserPlugin.Models.Config;
using ThreadSafeRandomizer;

namespace MapChooserPlugin.Helpers;

public class RandomizationHelper : AbstractBaseHelper, IRandomizationHelper
{
    private readonly MapChooserConfig _config;

    public List<string> ExcludedMaps { get; } = [];

    public RandomizationHelper(ILogHelper logHelper, MapChooserConfig config) : base(logHelper)
    {
        _config = config;
    }

    public string GetRandomizedMap()
    {
        var availableMaps = GetShuffledAvailableMaps();
        var serializedAvailableMaps = JsonSerializer.Serialize(availableMaps);
        LogHelper.LogTrace($"RandomizationHelper.GetRandomizedMap: serializedAvailableMaps: {serializedAvailableMaps}");
        var randomMap = availableMaps[ThreadSafeRandom.Instance.Next(0, availableMaps.Count)];
        LogHelper.LogTrace($"RandomizationHelper.GetRandomizedMap: randomMap: {randomMap}");
        return randomMap;
    }

    public List<string> GetShuffledAvailableMaps()
    {
        var availableMaps = _config.Maps.Where(map => ExcludedMaps.All(excluded => map != excluded)).ToList();
        var serializedAvailableMaps = JsonSerializer.Serialize(availableMaps);
        LogHelper.LogTrace($"RandomizationHelper.GetShuffledAvailableMaps: serializedAvailableMaps: {serializedAvailableMaps}");
        availableMaps.Shuffle();
        var serializedShuffledMaps = JsonSerializer.Serialize(availableMaps);
        LogHelper.LogTrace($"RandomizationHelper.GetShuffledAvailableMaps: serializedAvailableMaps: {serializedShuffledMaps}");
        return availableMaps;
    }

    public void ExcludeMap(string mapName)
    {
        LogHelper.LogTrace($"RandomizationHelper.ExcludeMap: mapName: {mapName}");
        ExcludedMaps.Add(mapName);
        if (ExcludedMaps.Count <= _config.ExcludeMapCount)
        {
            return;
        }

        ExcludedMaps.RemoveAt(0);
    }
}