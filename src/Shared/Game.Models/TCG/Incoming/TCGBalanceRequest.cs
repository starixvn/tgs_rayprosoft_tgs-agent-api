using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.TCG.Common;

namespace TGS.Shared.Game.Models.TCG.Incoming
{
    public record TCGBalanceRequest : TCGSingleWalletRequest
    {
        [Required]
        [JsonPropertyName("username")]
        public string username { get; init; }
        [Required]
        [JsonPropertyName("product_type")]
        public int ProductType { get; init; }

    }
}
