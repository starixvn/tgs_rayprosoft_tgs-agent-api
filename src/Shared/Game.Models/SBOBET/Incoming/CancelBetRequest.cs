using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record CancelBetRequest : BaseRequest
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; init; }

        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("IsCancelAll")]
        public bool IsCancelAll { get; init; }
    }
}