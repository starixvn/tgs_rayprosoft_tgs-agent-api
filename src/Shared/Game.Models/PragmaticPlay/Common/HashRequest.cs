using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Common
{
    public record HashRequest
    {
        [JsonPropertyName("hash")]
        public string Hash { get; init; }
    }
}