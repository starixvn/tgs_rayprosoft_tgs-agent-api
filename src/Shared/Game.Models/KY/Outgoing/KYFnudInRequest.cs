using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.KY.Commons;

namespace TGS.Shared.Game.Models.KY.Outgoing
{
    public record KYFnudInRequest : KYBaseRequest
    {
        [JsonPropertyName("account")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("money")]
        [Column(TypeName = "decimal(12, 2)")]
        public decimal Amount { get; init; }

        [JsonPropertyName("orderid")]
        public string OrderId { get; init; }
    }
}
