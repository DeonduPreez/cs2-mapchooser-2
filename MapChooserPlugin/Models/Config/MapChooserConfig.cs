using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Logging;

namespace MapChooserPlugin.Models.Config;

public class MapChooserConfig : BasePluginConfig
{
    [JsonPropertyName("LogLevel")]
    public LogLevel LogLevel { get; set; } = LogLevel.Information;

    [JsonPropertyName("AllowRtv")]
    public bool AllowRtv { get; set; } = true;

    [JsonPropertyName("AllowSpectatorRtv")]
    public bool AllowSpectatorRtv { get; set; } = false;

    [JsonPropertyName("RTVPercent")]
    public float RtvPercent { get; set; } = 0.6f;

    [JsonPropertyName("DelayRtv")]
    public bool DelayRtv { get; set; } = true;

    [JsonPropertyName("RTVDelaySeconds")]
    public int RtvDelaySeconds { get; set; } = 15;

    [JsonPropertyName("VoteOnRoundsBeforeMapEnd")]
    public int VoteOnRoundsBeforeMapEnd { get; set; } = 3;

    [JsonPropertyName("ExcludeMapCount")]
    public int ExcludeMapCount { get; set; } = 2;

    [JsonPropertyName("VoteDurationSeconds")]
    public int VoteDurationSeconds { get; set; } = 15;

    // TODO : Implement
    [JsonPropertyName("AllowNomination")]
    public bool AllowNomination { get; set; } = true;

    // TODO : Implement
    [JsonPropertyName("NominationExcludeCount")]
    public int NominationExcludeCount { get; set; } = 1;

    [JsonPropertyName("Maps")]
    public List<string> Maps { get; set; } =
    [
        "de_mirage",
        "de_inferno",
        "de_vertigo",
        "de_nuke",
        "de_ancient",
        "de_anubis",
        "de_overpass"
    ];
}