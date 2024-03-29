namespace MapChooserPlugin.Interfaces;

public interface IVoteHelper
{
    public bool Voting { get; }

    public void StartMapVoting(bool triggeredByRtv);

    #region Listeners

    public void OnMapStart(string mapName);

    #endregion
}