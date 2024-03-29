using System.Text.Json;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Menu;
using CounterStrikeSharp.API.Modules.Timers;
using ListShuffle;
using MapChooserPlugin.Classes;
using MapChooserPlugin.Classes.Abstract;
using MapChooserPlugin.Helpers.Static;
using MapChooserPlugin.Interfaces;
using MapChooserPlugin.Models.Config;
using ThreadSafeRandomizer;

namespace MapChooserPlugin.Helpers;

public class VoteHelper : AbstractBaseHelper, IVoteHelper
{
    private readonly MapChooserConfig _config;
    private readonly IChatHelper _chatHelper;
    private readonly IMapChangeHelper _mapChangeHelper;
    private readonly IRandomizationHelper _randomizationHelper;

    public bool Voting { get; private set; } = false;

    private bool _triggeredByRtv = false;
    private bool _forceMapVote = false;
    private string? _currentMap;
    private int _totalVoteCount = 0;
    private Dictionary<string, int> _votes = new();

    public VoteHelper(ILogHelper logHelper, MapChooserConfig config, IChatHelper chatHelper, IMapChangeHelper mapChangeHelper, IRandomizationHelper randomizationHelper) : base(logHelper)
    {
        _config = config;
        _chatHelper = chatHelper;
        _mapChangeHelper = mapChangeHelper;
        _randomizationHelper = randomizationHelper;

        switch (_config.VoteOnRoundsBeforeMapEnd)
        {
            case < 0:
                throw ThrowHelper.GetConfigurationException(nameof(MapChooserConfig.VoteOnRoundsBeforeMapEnd), "Cannot be less than 0");
            case < 1:
                LogHelper.LogConfigurationWarning(nameof(MapChooserConfig.VoteOnRoundsBeforeMapEnd), "It is not recommended to set this to a value less than 1");
                break;
        }

        var allMapsCount = _config.Maps.Count;
        if (_config.ExcludeMapCount >= allMapsCount)
        {
            throw ThrowHelper.GetConfigurationException(nameof(MapChooserConfig.ExcludeMapCount), $"Cannot be more than or equal to the map count ({allMapsCount})");
        }
    }

    public void StartMapVoting(bool triggeredByRtv)
    {
        if (Voting)
        {
            if (_triggeredByRtv)
            {
                _forceMapVote = true;
            }
            return;
        }

        LogHelper.LogTrace($"VoteHelper.StartMapVoting: triggeredByRtv: {triggeredByRtv}");
        Voting = true;
        _triggeredByRtv = triggeredByRtv;
        _totalVoteCount = 0;
        _votes.Clear();

        var availableOptions = _randomizationHelper.GetShuffledAvailableMaps();

        if (triggeredByRtv)
        {
            availableOptions.Insert(0, MapChooserPlugin.Instance.Localizer[MapChooserConstants.Translations.RtvOptionDontChange]);
        }

        var serializedAvailableOptions = JsonSerializer.Serialize(availableOptions);
        LogHelper.LogTrace($"VoteHelper.StartMapVoting: serializedAvailableOptions: {serializedAvailableOptions}");

        var chatMenu = new ChatMenu($"Map Voting has started - Type \"{CoreConfig.PublicChatTrigger}#\" to vote for a map");

        foreach (var option in availableOptions)
        {
            chatMenu.AddMenuOption(option, (controller, opt) =>
            {
                if (!Voting)
                {
                    return;
                }

                if (_votes.TryGetValue(opt.Text, out var count))
                {
                    _votes[opt.Text] = count + 1;
                }
                else
                {
                    _votes[opt.Text] = 1;
                }

                _totalVoteCount++;
                _chatHelper.PrintToChatAll(MapChooserPlugin.Instance.Localizer[MapChooserConstants.Translations.VotedForMap, controller.PlayerName, opt.Text]);
            });
        }

        foreach (var player in PlayerHelper.GetActivePlayers())
        {
            MenuManager.OpenChatMenu(player, chatMenu);
        }

        MapChooserPlugin.Instance.AddTimer(Math.Min(_config.VoteDurationSeconds, 60f), ConcludeMapVote, TimerFlags.STOP_ON_MAPCHANGE);
    }

    #region Listeners

    public void OnMapStart(string mapName)
    {
        _currentMap = mapName;

        _mapChangeHelper.SetNextMap(null);
    }

    #endregion

    #region Timer Methods

    private void ConcludeMapVote()
    {
        Voting = false;
        _triggeredByRtv = false;
        string nextMap;
        if (_totalVoteCount == 0)
        {
            if (_triggeredByRtv)
            {
                _chatHelper.PrintToChatAll(MapChooserPlugin.Instance.Localizer[MapChooserConstants.Translations.RtvFailedNotEnoughVotes]);
                if (_forceMapVote)
                {
                    StartMapVoting(false);
                }

                return;
            }

            nextMap = _randomizationHelper.GetRandomizedMap();
        }
        else
        {
            nextMap = GetVoteWinner();
        }

        _mapChangeHelper.SetNextMap(nextMap);
    }

    private string GetVoteWinner()
    {
        var winners = new List<string>();
        var winnerCount = 0;
        foreach (var (mapName, count) in _votes)
        {
            if (count == 0)
            {
                continue;
            }

            if (winners.Count == 0 || winnerCount > count)
            {
                winners.Clear();
                winners.Add(mapName);
                winnerCount = count;
                continue;
            }

            if (winnerCount == count)
            {
                winners.Add(mapName);
            }
        }

        if (winners.Count == 1)
        {
            return winners[0];
        }

        winners.Shuffle();
        return winners[ThreadSafeRandom.Instance.Next(0, winners.Count)];
    }

    #endregion
}