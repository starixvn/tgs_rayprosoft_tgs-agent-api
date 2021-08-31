using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.Shared.Game.Models.TCG.Outgoing
{
    public record TCGLaunchGameRequest
    {
        [Required]
        [JsonPropertyName("method")]
        public int Method { get; init; }
        [Required]
        [JsonPropertyName("username")]
        public string UserName { get; init; }
        [Required]
        [JsonPropertyName("product_type")]
        public int ProductType { get; init; }
        [Required]
        [JsonPropertyName("platform")]
        public string Platform { get; set; } = "html5-desktop";
        [Required]
        [JsonPropertyName("game_mode")]
        public string GameMode { get; init; }
        [Required]
        [JsonPropertyName("game_code")]
        public string GameCode { get; init; } = "Lobby";
    }
}
