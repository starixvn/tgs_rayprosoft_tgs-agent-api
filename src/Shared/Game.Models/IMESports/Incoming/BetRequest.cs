using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Incoming
{
    public record BetRequest : Base
    {
        [JsonPropertyName("SessionToken")]
        public string SessionToken { get; init; }
        
        [JsonPropertyName("Transactions")]
        public TransactionRequest[] Transactions { get; init; }
    }
}