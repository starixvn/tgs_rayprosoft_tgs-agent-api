using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record CancelBetRequest : TransactionRequest
    {
        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("IsCancelAll")]
        public bool IsCancelAll { get; init; }
    }
}