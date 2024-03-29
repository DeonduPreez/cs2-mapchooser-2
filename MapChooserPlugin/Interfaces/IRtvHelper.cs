using CounterStrikeSharp.API.Core;

namespace MapChooserPlugin.Interfaces;

public interface IRtvHelper
{
    public bool RtvEnabled { get; }

    public void ClearVotes();
    public void PlayerRtvVote(CCSPlayerController player);
    public void PlayerRtvRetractVote(CCSPlayerController player);

    #region Listeners

    public void OnMapStart();

    #endregion
}