using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.TCG.Common;

namespace TGS.Shared.Game.Models.TCG.Incoming
{
    public record TCGCancelReserveRequest : TCGSingleWalletRequest
    {
        [Required]
        [JsonPropertyName("ref_nos")]
        public string[] RefNos { get; init; }
        [Required]
        [JsonPropertyName("reservation_details")]
        public ReservationDetail[] ReservationDetails { get; init; }
    }
}
