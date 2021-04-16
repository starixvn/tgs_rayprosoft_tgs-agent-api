using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.PragmaticPlay.Incoming
{
    public record PromoWinRequest : BaseRequest
    {
        [JsonPropertyName("userId")]
        public string UserId { get; init; }

        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; }

        [JsonPropertyName("campaignId")]
        public string CampaignId { get; init; }

        [JsonPropertyName("campaignType")]
        public string CampaignType { get; init; }

        [JsonPropertyName("reference")]
        public string Reference { get; init; }

        [JsonPropertyName("timestamp")]
        public long Timestamp { get; init; }
    }
}