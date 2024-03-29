using MapChooserPlugin.Classes.Abstract;
using MapChooserPlugin.Interfaces;
using MapChooserPlugin.Models.Config;

namespace MapChooserPlugin.Helpers;

public class RoundHelper : AbstractBaseHelper, IRoundHelper
{
    private readonly MapChooserConfig _config;
    private readonly IMapChangeHelper _mapChangeHelper;
    private readonly IVoteHelper _voteHelper;

    private readonly int _maxRounds;

    private int _currentRound;
    private int _roundsLeft;

    public RoundHelper(ILogHelper logHelper, MapChooserConfig config, IMapChangeHelper mapChangeHelper, IVoteHelper voteHelper, IConVarHelper conVarHelper) : base(logHelper)
    {
        _config = config;
        _mapChangeHelper = mapChangeHelper;
        _voteHelper = voteHelper;

        _maxRounds = conVarHelper.GetMaxRounds();
        _currentRound = 0;
        _roundsLeft = _maxRounds;

        if (_config.VoteOnRoundsBeforeMapEnd >= 1)
        {
            return;
        }

        LogHelper.LogConfigurationWarning(nameof(MapChooserConfig.VoteOnRoundsBeforeMapEnd), "It is not recommended to set this to a value less than 1");
    }

    #region Public Helper Methods

    public bool ShouldStartVoting()
    {
        if (_voteHelper.Voting)
        {
            return false;
        }

        if (!string.IsNullOrWhiteSpace(_mapChangeHelper.NextMap))
        {
            return false;
        }

        return _config.VoteOnRoundsBeforeMapEnd >= _roundsLeft;
    }

    #endregion

    #region Listeners

    public void OnMapStart()
    {
        LogHelper.LogTrace("RoundHelper.OnMapStart");
        _currentRound = 0;
        _roundsLeft = _maxRounds;
    }

    #endregion

    #region Events

    public void OnRoundStart()
    {
        _currentRound++;
        _roundsLeft = _maxRounds - _currentRound;
        LogHelper.LogTrace($"RoundHelper.OnRoundStart: mapName: {_currentRound}. _roundsLeft: {_roundsLeft}");
        
        if (ShouldStartVoting())
        {
            _voteHelper.StartMapVoting(false);
        }
    }

    public void LastPlayerDisconnected()
    {
        LogHelper.LogTrace($"RoundHelper.LastPlayerDisconnected: mapName: {_currentRound}. _roundsLeft: {_roundsLeft}");
        _currentRound = 0;
        _roundsLeft = _maxRounds - _currentRound;
    }

    #endregion
}