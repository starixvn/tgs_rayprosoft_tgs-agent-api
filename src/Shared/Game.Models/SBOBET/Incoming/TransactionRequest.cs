using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Incoming
{
    public record TransactionRequest : BaseRequest
    {
        [JsonPropertyName("transferCode")]
        public string ExternalTransactionId { get; init; }
    }
}