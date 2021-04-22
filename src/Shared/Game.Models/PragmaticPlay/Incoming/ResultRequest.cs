using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record ResultRequest : TransactionRequest
    {
        [JsonPropertyName("bonusCode")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string BonusCode { get; init; }

        [JsonPropertyName("roundDetails")]
        public string RoundDetails { get; init; }

        [JsonPropertyName("promoCampaignID")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string PromoCampaignId { get; init; }

        [JsonPropertyName("promoCampaignType")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string PromoCampaignType { get; init; }

        [JsonPropertyName("promoWinAmount")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal? PromoWinAmount { get; init; }

        [JsonPropertyName("promoWinReference")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string PromoWinReference { get; init; }
    }
}