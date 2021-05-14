using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.AgentApi.Models
{
    public record AuthResponse : TgsResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; init; }

        /// <summary>
        ///     unix timestamp to milliseconds
        /// </summary>
        [JsonPropertyName("expires")]
        public long ExpiredOn { get; init; }
    }
}