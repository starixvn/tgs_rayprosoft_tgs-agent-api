using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.KY.Outgoing
{
    public record KYGameUrlRequest : KYBaseRequest
    {
        [JsonPropertyName("account")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("money")]
        public decimal Amount { get; init; }

        [JsonPropertyName("orderid")]
        public string Orderid { get; init; }

        [JsonPropertyName("ip")]
        public string IP { get; init; }

        [JsonPropertyName("lineCode")]
        public string BrandsCode { get; init; }

        [JsonPropertyName("KindID")]
        public int ExternalID { get; init; }
    }
}
