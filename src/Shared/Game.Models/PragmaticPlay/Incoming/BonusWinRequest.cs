using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record BonusWinRequest : TransactionRequest
    {
        [JsonPropertyName("bonusCode")]
        public string BonusCode { get; init; }
    }
}