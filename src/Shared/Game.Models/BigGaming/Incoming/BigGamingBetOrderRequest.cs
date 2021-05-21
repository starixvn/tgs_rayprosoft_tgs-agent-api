using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.BigGaming.Incoming
{
    public record BigGamingBetOrderRequest
    {
        [JsonPropertyName("orderId")]
        public int OrderId { get; set; }
        [JsonPropertyName("gameId")]
        public int ExternalGameId { get; set; }
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
        [JsonPropertyName("issueId")]
        public string IssueId { get; set; }
        [JsonPropertyName("playId")]
        public string PlayId { get; set; }
        [JsonPropertyName("orderFrom")]
        public int OrderFrom { get; set; }
        [JsonPropertyName("fromIp")]
        public string FromIp { get; set; }
    }
}
