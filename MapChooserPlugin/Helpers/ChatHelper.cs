using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using MapChooserPlugin.Classes;
using MapChooserPlugin.Classes.Abstract;
using MapChooserPlugin.Interfaces;

namespace MapChooserPlugin.Helpers;

public class ChatHelper : AbstractBaseHelper, IChatHelper
{
    public ChatHelper(ILogHelper logHelper) : base(logHelper)
    {
    }

    public void PrintToChatAll(string message)
    {
        LogHelper.LogTrace($"ChatHelper.PrintToChatAll: message: {message}");
        Server.PrintToChatAll($"{MapChooserPlugin.Instance.Localizer[MapChooserConstants.Translations.Prefix]} {message}");
    }

    public void PrintToChatPlayer(CCSPlayerController player, string message)
    {
        LogHelper.LogTrace($"ChatHelper.PrintToChatAll: message: {message}");
        player.PrintToChat($"{MapChooserPlugin.Instance.Localizer[MapChooserConstants.Translations.Prefix]} {message}");
    }
}