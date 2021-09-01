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
    public record TCGCheckTransactionStatusRequest : TCGBaseRequest
    {
        [Required]
        [JsonPropertyName("product_type")]
        public int ProductType { get; init; }

        [Required]
        [JsonPropertyName("ref_nos")]
        public string[] RefNos { get; init; }
        [Required]
        [JsonPropertyName("transaction_details")]
        public TransactionInfo[] TransactionDetails { get; init; }

        [Required]
        [JsonPropertyName("request_time")]
        public long RequestTime { get; init; }
    }
}
