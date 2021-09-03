using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.TCG.Common;

namespace TGS.Shared.Game.Models.TCG.Outgoing
{
    public record TCGRegisterPlayerRequest
    {
        [Required]
        [JsonPropertyName("method")]
        public string Method => "cm";
        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; init; }
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; init; }
    }
}
