using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Outgoing
{
    public record HashRequest
    {
        [JsonPropertyName("hash")]
        public string Hash { get; init; }
    }
}