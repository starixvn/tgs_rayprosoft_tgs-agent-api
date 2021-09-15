using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.TCG.Common;

namespace TGS.Shared.Game.Models.TCG.Outgoing
{
    public record TCGLaunchGameResponse : TCGBaseResponse
    {
        [JsonPropertyName("game_url")]
        public string GameUrl { get; init; }
        [JsonPropertyName("pt_username")]
        public string PTUsername { get; init; }
        [JsonPropertyName("pt_password")]
        public string PTPassword { get; init; }
    }
}
