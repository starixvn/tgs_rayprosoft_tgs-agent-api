using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRKickRequest
    {
        [JsonPropertyName("playerName")]
        public string PlayerCode { get; init; }
    }
}
