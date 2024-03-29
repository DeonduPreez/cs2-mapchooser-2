using CounterStrikeSharp.API.Core;

namespace MapChooserPlugin.Interfaces;

public interface IChatHelper
{
    void PrintToChatAll(string message);
    void PrintToChatPlayer(CCSPlayerController player, string message);
}