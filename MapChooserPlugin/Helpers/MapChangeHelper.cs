using CounterStrikeSharp.API;
using MapChooserPlugin.Classes;
using MapChooserPlugin.Classes.Abstract;
using MapChooserPlugin.Interfaces;

namespace MapChooserPlugin.Helpers;

public class MapChangeHelper : AbstractBaseHelper, IMapChangeHelper
{
    private readonly IChatHelper _chatHelper;
    private readonly IRandomizationHelper _randomizationHelper;
    private readonly int _matchRestartDelay;

    public MapChangeHelper(ILogHelper logHelper, IConVarHelper conVarHelper, IChatHelper chatHelper, IRandomizationHelper randomizationHelper) : base(logHelper)
    {
        _chatHelper = chatHelper;
        _randomizationHelper = randomizationHelper;

        _matchRestartDelay = conVarHelper.GetMatchRestartDelay();
    }

    public string? NextMap { get; private set; }

    public void OnMatchEnd()
    {
        LogHelper.LogTrace("MapChangeHelper.OnMatchEnd");
        if (_matchRestartDelay == MapChooserConstants.Miscellaneous.InstantRestartDelay)
        {
            LogHelper.LogTrace("MapChangeHelper.OnMatchEnd InstantRestart");
            ChangeMap();
            return;
        }

        MapChooserPlugin.Instance.AddTimer(_matchRestartDelay - 1, ChangeMap);
    }

    public void SetNextMap(string? nextMap)
    {
        LogHelper.LogTrace($"MapChangeHelper.SetNextMap: nextMap: {nextMap}");
        NextMap = nextMap;
        if (NextMap != null)
        {
            _chatHelper.PrintToChatAll(MapChooserPlugin.Instance.Localizer[MapChooserConstants.Translations.NextMapSelected]);
        }
    }
    
    private void ChangeMap()
    {
        LogHelper.LogTrace($"MapChangeHelper.ChangeMap: NextMap: {NextMap}");
        if (NextMap == null)
        {
            NextMap = _randomizationHelper.GetRandomizedMap();
            LogHelper.LogTrace($"MapChangeHelper.ChangeMap: Randomized NextMap: {NextMap}");
        }

        if (NextMap.StartsWith(MapChooserConstants.Miscellaneous.WorkShopMapPrefix))
        {
            Server.ExecuteCommand($"ds_workshop_changelevel {NextMap}");
            return;
        }

        Server.ExecuteCommand($"changelevel {NextMap}");
    }
}