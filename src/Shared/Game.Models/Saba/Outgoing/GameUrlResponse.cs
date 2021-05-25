using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.Saba.Enums;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record GameUrlResponse
    {
        [JsonPropertyName("error_code")]
        public GetGameUrlApi Code { get; init; }

        [JsonPropertyName("message")]
        public string ErrorMessage { get; init; }

        [JsonPropertyName("Data")]
        public string Url { get; init; }
    }
}