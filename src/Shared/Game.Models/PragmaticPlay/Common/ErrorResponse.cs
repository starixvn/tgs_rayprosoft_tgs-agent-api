using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Common
{
    public record ErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}