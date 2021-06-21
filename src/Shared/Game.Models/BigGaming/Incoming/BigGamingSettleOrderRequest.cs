using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Enums;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BigGamingSettleOrderRequest
    {
        [JsonPropertyName("gameId")]
        public int ExternalGameId { get; set; }
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }
        [JsonPropertyName("orderAmount")]
        public decimal OrderAmount { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("orderStatus")]
        public BigGamingOrderStatus OrderStatus { get; set; }
        [JsonPropertyName("validAmount")]
        public decimal ValidAmount { get; set; }
        [JsonPropertyName("orderFrom")]
        public int OrderFrom { get; set; }
    }
}
