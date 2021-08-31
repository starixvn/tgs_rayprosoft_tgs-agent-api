using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.TCG.Common;

namespace TGS.Shared.Game.Models.TCG.Outgoing
{
    public record TCGKickOutLottoPlayerRequest : TCGBaseRequest
    {
        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; init; }
    }
}
