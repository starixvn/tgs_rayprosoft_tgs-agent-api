using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record PlaceBetRequest : TransactionRequest
    {
        [JsonPropertyName("bonusCode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string BonusCode { get; init; }

        [JsonPropertyName("roundDetails")]
        public string RoundDetails { get; init; }

        [JsonPropertyName("jackpotId")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string JackpotId { get; init; }

        [JsonPropertyName("jackpotContribution")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal? JackpotContribution { get; init; }

        [JsonPropertyName("jackpotDetails")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string JackpotDetails { get; init; }
    }
}