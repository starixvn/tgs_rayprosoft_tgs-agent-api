using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.TCG.Common;

namespace TGS.Shared.Game.Models.TCG.Outgoing
{
    public record TCGLaunchGameRequest
    {
        [Required]
        [JsonPropertyName("method")]
        public string Method => "lg";
        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; init; }
        [Required]
        [JsonPropertyName("platform")]
        public string Platform { get; set; } = "html5-desktop";
        [Required]
        [JsonPropertyName("game_mode")]
        public string GameMode { get; init; }
        [Required]
        [JsonPropertyName("game_code")]
        public string GameCode { get; set; } = "Lobby";
        [Required]
        [JsonPropertyName("language")]
        public string Language { get; init; }
    }
}
