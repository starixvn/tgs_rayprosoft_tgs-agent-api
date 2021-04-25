using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.JDB.Enums;

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record BaseRequest
    {
        public string x { get; set; }
    }

    public record BaseActionRequest
    {
        [JsonPropertyName("action")]
        public ActionType ActionType { get; set; }

        [JsonPropertyName("ts")]
        public long SystemTime { get; set; }

        [JsonPropertyName("uid")]
        public string UserId { get; set; }
    }
}