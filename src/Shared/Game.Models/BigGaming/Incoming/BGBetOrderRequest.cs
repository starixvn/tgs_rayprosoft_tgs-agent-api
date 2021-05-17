using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BGBetOrderRequest
    {
        [JsonPropertyName("gameId")]
        public int GameId { get; set; }
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }
        [JsonPropertyName("orderFrom")]
        public int OrderFrom { get; set; }
        [JsonPropertyName("amount")]
        public int Amount { get; set; }
    }
}
