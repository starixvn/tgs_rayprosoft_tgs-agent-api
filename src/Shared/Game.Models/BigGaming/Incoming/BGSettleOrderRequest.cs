using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BGSettleOrderRequest
    {
        [JsonPropertyName("gameId")]
        public int GameId { get; set; }
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }
        [JsonPropertyName("orderFrom")]
        public int OrderFrom { get; set; }
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
        [JsonPropertyName("orderStatus")]
        public int OrderStatus { get; set; }
        [JsonPropertyName("orderAmount")]
        public int OrderAmount { get; set; }
    }
}
