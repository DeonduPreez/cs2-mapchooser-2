using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;

namespace MapChooserPlugin.Helpers.Static;

public static class PlayerHelper
{
    public static IEnumerable<CCSPlayerController> GetActivePlayers(bool allowSpectator = false)
    {
        var players = Utilities.GetPlayers().Where(player =>
        {
            if (!allowSpectator && player.Team <= CsTeam.Spectator)
            {
                return false;
            }

            if (player.IsBot)
            {
                return false;
            }

            return player.IsValid && player.Connected == PlayerConnectedState.PlayerConnected;
        });
        return players;
    }

    public static int GetActivePlayerCount(bool allowSpectator = false)
    {
        return GetActivePlayers(allowSpectator).Count();
    }
}