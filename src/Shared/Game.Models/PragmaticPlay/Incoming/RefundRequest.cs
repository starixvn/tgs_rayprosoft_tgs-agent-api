using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record RefundRequest : TransactionRequest
    {
        [JsonPropertyName("bonusCode")]
        public string BonusCode { get; init; }

        [JsonPropertyName("roundDetails")]
        public string RoundDetails { get; init; }
    }
}