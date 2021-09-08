using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.TCG.Enums;

namespace TGS.Shared.Game.Models.TCG.Common
{
    public class TCGTransactionData
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
        public TCGTransactionType TxType { get; init; }
        [JsonPropertyName("remarks")]
        public string Remarks { get; init; }
        [JsonPropertyName("additionalInformation")]
        public JsonDocument AdditionalInformation { get; init; }
    }
}
