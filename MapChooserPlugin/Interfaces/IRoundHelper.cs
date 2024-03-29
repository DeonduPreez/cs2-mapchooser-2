namespace MapChooserPlugin.Interfaces;

public interface IRoundHelper
{
    public bool ShouldStartVoting();

    #region Listeners

    public void OnMapStart();

    #endregion

    #region Events

    public void OnRoundStart();
    public void LastPlayerDisconnected();

    #endregion
}