using CounterStrikeSharp.API.Modules.Cvars;
using MapChooserPlugin.Classes;
using MapChooserPlugin.Classes.Abstract;
using MapChooserPlugin.Helpers.Static;
using MapChooserPlugin.Interfaces;

namespace MapChooserPlugin.Helpers;

public class ConVarHelper : AbstractBaseHelper, IConVarHelper
{
    public ConVarHelper(ILogHelper logHelper) : base(logHelper)
    {
    }

    public int GetMaxRounds()
    {
        LogHelper.LogTrace("ConVarHelper.GetMaxRounds");
        var maxRoundsConVar = ConVar.Find(MapChooserConstants.ConVars.MaxRoundsCvar);
        if (maxRoundsConVar == null)
        {
            throw ThrowHelper.GetConVarNotFoundException(MapChooserConstants.ConVars.MaxRoundsCvar);
        }

        var maxRoundsInt = maxRoundsConVar.GetPrimitiveValue<int>();
        LogHelper.LogTrace($"ConVarHelper.GetMaxRounds: maxRoundsInt: {maxRoundsInt}");
        return maxRoundsInt;
    }

    public int GetMatchRestartDelay()
    {
        var matchRestartDelayConVar = ConVar.Find(MapChooserConstants.ConVars.MatchRestartDelayCvar);
        var matchRestartDelayInt = matchRestartDelayConVar?.GetPrimitiveValue<int>() ?? MapChooserConstants.Miscellaneous.InstantRestartDelay;
        LogHelper.LogTrace($"ConVarHelper.GetMatchRestartDelay: matchRestartDelayInt: {matchRestartDelayInt}");
        return matchRestartDelayInt;
    }
}