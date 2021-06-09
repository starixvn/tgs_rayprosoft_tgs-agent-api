using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.SexyGaming.Outgoing
{
    public record SexyGamingGameRecord
    {
        [JsonPropertyName("result")]
        public string[] Result { get; set; }
        [JsonPropertyName("roundStartTime")]
        public string RoundStartTime { get; set; }
        [JsonPropertyName("winner")]
        public string Winner { get; set; }
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
        [JsonPropertyName("odds")]
        public decimal Odds { get; set; }
        [JsonPropertyName("tableId")]
        public int TableId { get; set; }
        [JsonPropertyName("dealerDomain")]
        public string DealerDomain { get; set; }
        [JsonPropertyName("winLoss")]
        public decimal WinLoss { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
