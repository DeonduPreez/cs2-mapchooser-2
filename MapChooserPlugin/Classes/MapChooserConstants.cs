namespace MapChooserPlugin.Classes;

public class MapChooserConstants
{
    public class ConVars
    {
        public const string MaxRoundsCvar = "mp_maxrounds";
        public const string MatchRestartDelayCvar = "mp_match_restart_delay";
    }

    public class Translations
    {
        public const string Prefix = "mapchooser.prefix";

        public const string RtvEnabled = "mapchooser.rtv_enabled";
        public const string RtvNotAllowed = "mapchooser.rtv_not_allowed";
        public const string RtvNotEnabled = "mapchooser.rtv_not_enabled";
        public const string RtvSpectatorNotAllowed = "mapchooser.rtv_spectator_not_allowed";
        public const string RtvVote = "mapchooser.rtv_vote";
        public const string RtvRetractVote = "mapchooser.rtv_retract_vote";
        public const string RtvMovedToSpectator = "mapchooser.rtv_moved_to_spectator";
        public const string RtvNotVotedYet = "mapchooser.rtv_not_voted_yet";
        public const string RtvFailedNotEnoughVotes = "mapchooser.rtv_failed_not_enough_votes";
        public const string RtvOptionDontChange = "mapchooser.rtv_option_dont_change";
        public const string VotedForMap = "mapchooser.voted_for_map";

        public const string NextMapSelected = "mapchooser.next_map_selected";
    }

    public class Miscellaneous
    {
        public const int InstantRestartDelay = -1;
        public const string WorkShopMapPrefix = "ws:";
    }
}