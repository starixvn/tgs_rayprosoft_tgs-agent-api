using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BigGaming.Commons
{
    public record BigGamingResultResponse
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        [JsonPropertyName("sn")]
        public string Sn { get; set; }
        [JsonPropertyName("availableAmount")]
        public decimal AvailableAmount { get; set; }
        [JsonPropertyName("orderResult")]
        public string OrderResult { get; set; }
        [JsonPropertyName("tranId")]
        public object TranId { get; set; }
    }
}
