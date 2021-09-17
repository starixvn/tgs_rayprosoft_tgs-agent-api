using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.VR.Outgoing
{
    public record VRKickResponse
    {
        [JsonPropertyName("errorCode")]
        public int ErrorCode { get; init; }
    }
}
