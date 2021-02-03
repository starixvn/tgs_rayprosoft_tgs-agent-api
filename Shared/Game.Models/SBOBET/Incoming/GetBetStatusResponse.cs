using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record GetBetStatusResponse
    {
        [JsonPropertyName("errorCode")]
        public int ErrorCode { get; init; }

        [JsonPropertyName("errorMessage")]
        public string ErrorMessage { get; init; }

        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; init; }

        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("status")]
        public string Status {  get; init; }

        [JsonPropertyName("winloss")]
        public decimal WinLoss { get; init; }

        [JsonPropertyName("stake")]
        public decimal Stake { get; init; }
    }
}