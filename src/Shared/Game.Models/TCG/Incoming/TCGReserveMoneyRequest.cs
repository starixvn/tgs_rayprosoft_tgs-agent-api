using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.TCG.Common;

namespace TGS.Shared.Game.Models.TCG.Incoming
{
    public record TCGReserveMoneyRequest : TCGSingleWalletRequest
    {
        [Required]
        [JsonPropertyName("product_type")]
        public int ProductType { get; init; }
        [Required]
        [JsonPropertyName("transactions")]
        public TransactionData[] Transactions { get; init; }
    }
}
