using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record Name
    {
        [JsonPropertyName("lang")]
        public string Language { get; init; }

        [JsonPropertyName("name")]
        public string Value { get; init; }
    }
}