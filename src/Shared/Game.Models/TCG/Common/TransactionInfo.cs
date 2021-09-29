using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TCG.Common
{
    public record TransactionInfo
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
        [JsonPropertyName("additionalInformation")]
        public JsonDocument AdditionalInformation { get; init; }
    }
}
