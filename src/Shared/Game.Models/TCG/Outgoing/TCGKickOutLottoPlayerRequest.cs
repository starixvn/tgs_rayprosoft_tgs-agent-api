using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.TCG.Outgoing
{
    public record TCGKickOutLottoPlayerRequest
    {
        [Required]
        [JsonPropertyName("method")]
        public int Method { get; init; }
        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; init; }
    }
}
