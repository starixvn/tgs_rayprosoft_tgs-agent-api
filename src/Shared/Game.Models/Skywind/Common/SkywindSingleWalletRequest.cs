using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Skywind.Common
{
    public record SkywindSingleWalletRequest
    {
        [Required]
        [JsonPropertyName("merch_id")]
        public string MerchId { get; set; }
        [Required]
        [JsonPropertyName("merch_pwd")]
        public string MerchPwd { get; set; }
    }
}
