using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Outgoing
{
    public record ErrorResponse
    {
        [JsonPropertyName("errorCode")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; init; }
    }
}