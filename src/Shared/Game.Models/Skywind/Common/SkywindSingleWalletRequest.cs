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
        [JsonPropertyName("merch_id")]
        public string MerchId { get; set; }
        [JsonPropertyName("merch_pwd")]
        public string MerchPwd { get; set; }
    }
}
