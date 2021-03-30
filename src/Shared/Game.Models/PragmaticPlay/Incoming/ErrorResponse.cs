using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.PragmaticPlay.Enums;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record ErrorResponse
    {
        [JsonPropertyName("error")]
        public GameApiError Error { get; init; } = GameApiError.Success;

        [JsonPropertyName("description")]
        public string Description { get; init; } = string.Empty;
    }
}