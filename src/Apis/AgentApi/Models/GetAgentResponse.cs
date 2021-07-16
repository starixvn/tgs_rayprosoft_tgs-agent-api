using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.AgentApi.Models
{
    public record GetAgentResponse : TgsResponse
    {
        [JsonPropertyName("data")]
        public AgentData Data { get; init; }
    }
}