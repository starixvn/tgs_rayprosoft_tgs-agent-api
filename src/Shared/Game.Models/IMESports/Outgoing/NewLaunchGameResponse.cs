// ReSharper disable IdentifierTypo

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.IMESports.Outgoing
{
    public record NewLaunchGameResponse : BaseResponse
    {
        [JsonPropertyName("GameUrl")]
        public string GameUrl { get; init; }
        
        [JsonPropertyName("SessionToken")]
        public string SessionToken { get; init; }
        
        [JsonPropertyName("TTL")]
        public int TimeToLeave { get; init; }
    }
}
