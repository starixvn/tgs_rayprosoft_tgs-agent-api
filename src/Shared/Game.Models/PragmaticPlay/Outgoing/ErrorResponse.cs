using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Outgoing
{
    public record ErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; init; }

        [JsonPropertyName("description")]
        public string Description { get; init; }
    }
}