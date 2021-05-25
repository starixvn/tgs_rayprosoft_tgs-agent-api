using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BigGamingBetResponse
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
