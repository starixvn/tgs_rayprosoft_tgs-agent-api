using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record AuthResponse : TgsResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; init; }

        [JsonPropertyName("expires")]
        public long ExpireTime { get; init; }
    }
}
