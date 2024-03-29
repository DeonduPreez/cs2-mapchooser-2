using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using MapChooserPlugin.Classes;
using MapChooserPlugin.Classes.Abstract;
using MapChooserPlugin.Helpers.Static;
using MapChooserPlugin.Interfaces;
using MapChooserPlugin.Models.Config;

namespace MapChooserPlugin.Helpers;

public class RtvHelper : AbstractBaseHelper, IRtvHelper
{
    private readonly MapChooserConfig _config;
    private readonly IChatHelper _chatHelper;
    private readonly IVoteHelper _voteHelper;
    public bool RtvEnabled { get; private set; }

    public List<ulong> RtvPlayers = [];

    public RtvHelper(ILogHelper logHelper, MapChooserConfig config, IChatHelper chatHelper, IVoteHelper voteHelper) : base(logHelper)
    {
        _config = config;
        _chatHelper = chatHelper;
        _voteHelper = voteHelper;
    }

    public void ClearVotes()
    {
        LogHelper.LogTrace("RtvHelper.ClearVotes");
        RtvPlayers.Clear();
    }

    public void PlayerRtvVote(CCSPlayerController player)
    {
        if (_voteHelper.Voting)
        {
            return;
        }

        var playerSteamId = player.SteamID;
        LogHelper.LogTrace($"RtvHelper.PlayerRtvVote: playerSteamId: {playerSteamId}");
        if (!RtvEnabled)
        {
            PrintRtvNotEnabled(player);
            return;
        }

        if (!_config.AllowSpectatorRtv && player.Team <= CsTeam.Spectator)
        {
            PrintSpectatorNotAllowed(player);
            return;
        }

        RtvPlayers.Add(playerSteamId);

        var requiredVotes = GetRequiredVoteCount();
        PrintRtvCount(player, MapChooserConstants.Translations.RtvVote, requiredVotes);
    }

    public void PlayerRtvRetractVote(CCSPlayerController player)
    {
        if (_voteHelper.Voting)
        {
            return;
        }

        var playerSteamId = player.SteamID;
        LogHelper.LogTrace($"RtvHelper.PlayerRtvUnvote: playerSteamId: {playerSteamId}");
        if (!RtvEnabled)
        {
            PrintRtvNotEnabled(player);
            return;
        }

        if (!_config.AllowSpectatorRtv && player.Team <= CsTeam.Spectator)
        {
            PrintSpectatorNotAllowed(player);
            return;
        }

        if (!RemovePlayerRtv(player))
        {
            _chatHelper.PrintToChatPlayer(player, MapChooserPlugin.Instance.Localizer[MapChooserConstants.Translations.RtvNotVotedYet]);
            return;
        }

        var requiredVotes = GetRequiredVoteCount();
        PrintRtvCount(player, MapChooserConstants.Translations.RtvRetractVote, requiredVotes);
    }

    #region Listeners

    public void OnMapStart()
    {
        LogHelper.LogTrace($"RtvHelper.OnMapStart: _config.AllowRtv: {_config.AllowRtv}");
        if (!_config.AllowRtv)
        {
            return;
        }

        LogHelper.LogTrace($"RtvHelper.OnMapStart: _config.DelayRtv: {_config.DelayRtv}. _config.RtvDelaySeconds: {_config.RtvDelaySeconds}");
        if (!_config.DelayRtv || _config.RtvDelaySeconds <= 10)
        {
            EnableRtv();
            return;
        }

        MapChooserPlugin.Instance.AddTimer(_config.RtvDelaySeconds, EnableRtv);
    }

    #endregion

    public bool RemovePlayerRtv(CCSPlayerController player)
    {
        if (!_config.AllowRtv)
        {
            return false;
        }

        var rtvPlayerIndex = RtvPlayers.FindIndex(rtvPlayerId => rtvPlayerId == player.SteamID);

        if (rtvPlayerIndex == -1)
        {
            return false;
        }

        RtvPlayers.RemoveAt(rtvPlayerIndex);
        return true;
    }

    public void OnPlayerMoveToSpectator(CCSPlayerController player)
    {
        if (_config.AllowSpectatorRtv)
        {
            return;
        }

        if (!RemovePlayerRtv(player))
        {
            return;
        }

        PrintRtvCount(player, MapChooserConstants.Translations.RtvMovedToSpectator, GetRequiredVoteCount());
    }

    private void EnableRtv()
    {
        LogHelper.LogTrace("RtvHelper.EnableRtv");
        RtvEnabled = true;
        _chatHelper.PrintToChatAll(MapChooserPlugin.Instance.Localizer[MapChooserConstants.Translations.RtvEnabled]);
    }

    private void PrintRtvNotEnabled(CCSPlayerController player)
    {
        var translatorValue = !_config.AllowRtv ? MapChooserConstants.Translations.RtvNotAllowed : MapChooserConstants.Translations.RtvNotEnabled;
        _chatHelper.PrintToChatPlayer(player, MapChooserPlugin.Instance.Localizer[translatorValue]);
    }

    private void PrintRtvCount(CCSPlayerController player, string translationKey, double requiredVotes)
    {
        _chatHelper.PrintToChatAll(MapChooserPlugin.Instance.Localizer[translationKey, player.PlayerName, RtvPlayers.Count, requiredVotes]);
    }

    private void PrintSpectatorNotAllowed(CCSPlayerController player)
    {
        _chatHelper.PrintToChatPlayer(player, MapChooserPlugin.Instance.Localizer[MapChooserConstants.Translations.RtvSpectatorNotAllowed]);
    }

    private double GetRequiredVoteCount()
    {
        return Math.Round(Math.Floor(PlayerHelper.GetActivePlayerCount() * _config.RtvPercent), 1);
    }
}