using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public abstract record TransactionResponse : BalanceResponse
    {
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; }
    }
}