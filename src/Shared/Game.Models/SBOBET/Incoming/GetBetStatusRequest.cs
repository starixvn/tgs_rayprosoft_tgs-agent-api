using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record GetBetStatusRequest : TransactionRequest
    {
        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; init; }
    }
}