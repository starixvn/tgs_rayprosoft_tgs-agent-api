using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TGS.Shared.Game.Models.TCG.Common;

namespace TGS.Shared.Game.Models.TCG.Incoming
{
    public record TCGCreditRequest : TCGBaseRequest
    {
        [Required]
        [JsonPropertyName("product_type")]
        public int ProductType { get; init; }
        [Required]
        [JsonPropertyName("transactions")]
        public TransactionData[] Transactions { get; init; }
        [JsonPropertyName("request_time")]
        public long RequestTime { get; init; }
    }
}
