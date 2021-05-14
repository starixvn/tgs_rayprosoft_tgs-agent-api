using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Incoming
{
    public record BetRequest : Base
    {
        [JsonPropertyName("Transactions")]
        public TransactionRequest[] Transactions { get; init; }
    }
}