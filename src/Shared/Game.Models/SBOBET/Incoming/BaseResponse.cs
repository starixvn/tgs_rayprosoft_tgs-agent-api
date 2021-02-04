using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record BaseResponse
    {
        [JsonPropertyName("errorCode")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; init; }

        [JsonPropertyName("accountName")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("balance")]
        public decimal? Balance { get; init; }
    }
}
