using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.AgentApi.Models
{
    public record LogoutResponse : TgsResponse
    {
        /// <summary>
        ///     unix timestamp to milliseconds
        /// </summary>
        [JsonPropertyName("updatetime")]
        public long UpdateTime { get; init; }
    }
}