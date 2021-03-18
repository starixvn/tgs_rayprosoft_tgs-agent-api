using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record GetBetStatusRequest : BaseRequest
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; init; }

        [JsonPropertyName("transactionId")]
        public string ExternalRoundId { get; init; }
    }
}