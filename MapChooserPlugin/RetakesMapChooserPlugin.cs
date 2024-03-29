using System.Text.Json;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Utils;
using MapChooserPlugin.Helpers;
using MapChooserPlugin.Helpers.Static;
using MapChooserPlugin.Models.Config;

namespace MapChooserPlugin;

[MinimumApiVersion(202)]
public class MapChooserPlugin : BasePlugin, IPluginConfig<MapChooserConfig>
{
    // TODO : Localization
    // TODO : Ensure workshop maps work
    public static MapChooserPlugin Instance { get; private set; } = null!;

    #region Plugin info

    private const string Version = "0.0.1";
    public override string ModuleName => "Map Chooser Plugin";
    public override string ModuleVersion => Version;
    public override string ModuleAuthor => "Metal Injection";
    public override string ModuleDescription => "https://github.com/DeonduPreez/MapChooser";

    #endregion

    #region Constants

    public const string LogPrefix = $"[MapChooser {Version}]";

    #endregion

    #region Configs

    public MapChooserConfig Config { get; set; } = null!;

    #endregion

    #region Helpers

    private LogHelper _logHelper = null!;
    private RandomizationHelper _randomizationHelper = null!;
    private ChatHelper _chatHelper = null!;
    private RtvHelper _rtvHelper = null!;
    private RoundHelper _roundHelper = null!;
    private VoteHelper _voteHelper = null!;
    private MapChangeHelper _mapChangeHelper = null!;
    private ConVarHelper _conVarHelper = null!;

    #endregion

    #region Setup

    public MapChooserPlugin()
    {
        Instance = this;
    }

    public override void Load(bool hotReload)
    {
        RegisterListener<Listeners.OnMapStart>(OnMapStart);

        RegisterEventHandler<EventRoundStart>(EventOnRoundStart);
        RegisterEventHandler<EventServerCvar>(EventOnCvarUpdated);
        RegisterEventHandler<EventCsWinPanelMatch>(OnMatchEndEvent);
        RegisterEventHandler<EventPlayerDisconnect>(EventOnPlayerDisconnect);
        RegisterEventHandler<EventPlayerTeam>(EventOnPlayerMoveTeam);

        base.Load(hotReload);
    }

    public void OnConfigParsed(MapChooserConfig config)
    {
        var serializedConfig = JsonSerializer.Serialize(config);
        _logHelper.LogTrace($"MapChooserPlugin.OnConfigParsed: config: {serializedConfig}");
        Config = config;
        _logHelper = new LogHelper(Config);
        _randomizationHelper = new RandomizationHelper(_logHelper, Config);
        _chatHelper = new ChatHelper(_logHelper);
        _conVarHelper = new ConVarHelper(_logHelper);
        _mapChangeHelper = new MapChangeHelper(_logHelper, _conVarHelper, _chatHelper, _randomizationHelper);
        _voteHelper = new VoteHelper(_logHelper, Config, _chatHelper, _mapChangeHelper, _randomizationHelper);
        _rtvHelper = new RtvHelper(_logHelper, Config, _chatHelper, _voteHelper);
        _roundHelper = new RoundHelper(_logHelper, Config, _mapChangeHelper, _voteHelper, _conVarHelper);
    }

    #endregion

    #region Listeners

    private void OnMapStart(string mapName)
    {
        _logHelper.LogTrace($"MapChooserPlugin.OnMapStart: mapName: {mapName}");
        _rtvHelper.OnMapStart();
        _roundHelper.OnMapStart();
        _voteHelper.OnMapStart(mapName);
        if (Config.ExcludeMapCount > 0)
        {
            _randomizationHelper.ExcludeMap(mapName);
        }
    }

    #endregion

    #region Event Handlers

    private HookResult EventOnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        var serializedEvent = JsonSerializer.Serialize(@event);
        var serializedInfo = JsonSerializer.Serialize(info);
        _logHelper.LogTrace($"MapChooserPlugin.EventOnRoundStart: Event: {serializedEvent}. Info: {serializedInfo}");
        _roundHelper.OnRoundStart();

        return HookResult.Continue;
    }

    private HookResult EventOnCvarUpdated(EventServerCvar @event, GameEventInfo info)
    {
        // TODO : Ensure Cvars are up to date
        var serializedEvent = JsonSerializer.Serialize(@event);
        var serializedInfo = JsonSerializer.Serialize(info);
        _logHelper.LogTrace($"MapChooserPlugin.EventOnCvarUpdated: Event: {serializedEvent}. Info: {serializedInfo}");
        return HookResult.Continue;
    }

    private HookResult OnMatchEndEvent(EventCsWinPanelMatch @event, GameEventInfo info)
    {
        _mapChangeHelper.OnMatchEnd();
        return HookResult.Continue;
    }

    private HookResult EventOnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        _rtvHelper.RemovePlayerRtv(@event.Userid);
        if (PlayerHelper.GetActivePlayerCount(true) == 0)
        {
            _roundHelper.LastPlayerDisconnected();
        }
        return HookResult.Continue;
    }

    private HookResult EventOnPlayerMoveTeam(EventPlayerTeam @event, GameEventInfo info)
    {
        if (@event.Team > (int)CsTeam.Spectator)
        {
            return HookResult.Continue;
        }

        _rtvHelper.OnPlayerMoveToSpectator(@event.Userid);

        return HookResult.Continue;
    }

    #endregion
}