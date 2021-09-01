using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TCG.Common
{
    public class TransactionData
    {
        [Required]
        [JsonPropertyName("ref_no")]
        public string RefNo { get; init; }
        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; init; }
        [Required]
        [JsonPropertyName("amount")]
        public decimal Amount { get; init; }
        [Required]
        [JsonPropertyName("tx_type")]
        public string TxType { get; init; }
        [JsonPropertyName("remarks")]
        public string Remarks { get; init; }
        [JsonPropertyName("additionalInformation")]
        public JsonDocument AdditionalInformation { get; init; }
    }
}
