using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.BigGaming.Enums;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BGSettleOrderRequest
    {
        [JsonPropertyName("gameId")]
        public int ExternalGameId { get; set; }
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }
        [JsonPropertyName("orderAmount")]
        public int OrderAmount { get; set; }
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
        [JsonPropertyName("orderStatus")]
        public BGOrderStatus OrderStatus { get; set; }
        [JsonPropertyName("validAmount")]
        public int ValidAmount { get; set; }
        [JsonPropertyName("orderFrom")]
        public int OrderFrom { get; set; }
    }
}
