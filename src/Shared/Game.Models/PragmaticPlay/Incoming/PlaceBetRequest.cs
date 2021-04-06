using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record PlaceBetRequest : TransactionRequest
    {
        [JsonPropertyName("bonusCode")]
        public string BonusCode { get; init; }

        [JsonPropertyName("roundDetails")]
        public string RoundDetails { get; init; }

        [JsonPropertyName("jackpotId")]
        public string JackpotId { get; init; }

        [JsonPropertyName("jackpotContribution")]
        public string JackpotContribution { get; init; }

        [JsonPropertyName("jackpotDetails")]
        public string JackpotDetails { get; init; }
    }
}