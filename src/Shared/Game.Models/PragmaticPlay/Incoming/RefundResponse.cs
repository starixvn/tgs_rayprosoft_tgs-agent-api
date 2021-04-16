using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record RefundResponse : ErrorResponse
    {
        [JsonPropertyName("transactionId")]
        public string TransactionId { get; init; }
    }
}