using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.JDB.Enums;

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record BaseActionRequest
    {
        [JsonPropertyName("action")]
        public ActionType ActionType { get; init; }

        [JsonPropertyName("ts")]
        public long SystemTime { get; init; }

        [JsonPropertyName("uid")]
        public string UserId { get; init; }
    }
}