using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BigGamingOrderStatusOrderMapRequest
    {
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; }
    }
}
