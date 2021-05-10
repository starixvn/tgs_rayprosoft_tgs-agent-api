using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record BetHistoryData
    {
        [JsonPropertyName("last_version_key")]
        public string VersionKey { get; init; }

        [JsonPropertyName("BetDetails")]
        public GameResult[] Results { get; init; }
    }
}