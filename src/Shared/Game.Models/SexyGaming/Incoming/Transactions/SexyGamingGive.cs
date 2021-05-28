using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.SexyGaming.Enums;

namespace TGS.Shared.Game.Models.SexyGaming.Incoming
{
    public record SexyGamingGive
    {
        [JsonPropertyName("txTime")]
        public string TxTime { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("promotionTxId")]
        public string PromotionTxId { get; set; }
        [JsonPropertyName("promotionId")]
        public int PromotionId { get; set; }
        [JsonPropertyName("promotionTypeId")]
        public int PromotionTypeId { get; set; }
        [JsonPropertyName("userId")]
        public string UserId { get; set; }
        [JsonPropertyName("platform")]
        public string Platform { get; set; }
    }
}
