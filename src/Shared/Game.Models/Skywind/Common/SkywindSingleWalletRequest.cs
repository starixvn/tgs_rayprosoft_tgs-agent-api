using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.Skywind.Common
{
    public record SkywindSingleWalletRequest
    {
        [JsonPropertyName("error_code")]
        public int ErrorCode { get; set; }
        [JsonPropertyName("error_msg")]
        public string ErrorMsg { get; set; }
        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }
        [JsonPropertyName("trx_id")]
        public string TrxId { get; set; }
        [JsonPropertyName("free_bet_count")]
        public int FreeBetCount { get; set; }
        [JsonPropertyName("messages")]
        public string[] Messages { get; set; }
    }


   
}
