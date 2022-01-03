using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Incoming
{
    public record ManuelRoundRecordResponse
    {
        [JsonPropertyName("errorCode")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; init; } = string.Empty;
    }
}
