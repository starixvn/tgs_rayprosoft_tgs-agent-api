using System;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.IMESports.Enums;

namespace TGS.Shared.Game.Models.IMESports.Incoming
{
    public record TransactionRequest : BaseRequest
    {
        [JsonPropertyName("GameId")]
        public string ExternalGameId { get; init; }
        
        [JsonPropertyName("BetId")]
        public string ExternalRoundId { get; init; }
        
        [JsonPropertyName("TransactionId")]
        public string ExternalTransactionId { get; init; }
        
        [JsonPropertyName("RefTransactionId")]
        public string[] RefTransactionIds { get; init; }

        [JsonPropertyName("ActionId")]
        public ActionType ActionType { get; init; }

        [JsonPropertyName("Amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("Platform")] public string Platform { get; init; } = "Desktop";
        
        [JsonPropertyName("TimeStamp")]
        public DateTimeOffset Timestamp { get; init; }
    }
}