using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Incoming
{
    public record BatchRequestTransaction
    {
        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("currency")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("commission")]
        public decimal Commission { get; init; }

        [JsonPropertyName("gameid")]
        public string ExternalGameId { get; init; }

        [JsonPropertyName("txid")]
        public string ExternalTransactionId { get; init; }

        [JsonPropertyName("txtype")]
        public int? GameActionType { get; init; }

        [JsonPropertyName("txrefid")]
        public string ExternalTransactionReferenceId { get; init; }

        [JsonPropertyName("roundid")]
        public string ExternalRoundId { get; init; }

        [JsonPropertyName("betid")]
        public string ExternalBetId { get; init; }

        [JsonPropertyName("roundclosed")]
        public bool? RoundClosed { get; init; }

        [JsonPropertyName("desc")]
        public string Description { get; init; }
    }
}