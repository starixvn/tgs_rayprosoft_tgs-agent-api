using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Incoming
{
    public record BaseRequest
    {
        [JsonPropertyName("key")]
        public string Key { get; init; }
    }
}