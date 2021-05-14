using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.AgentApi.Models
{
    public record SuccessResponse<TResult> : TgsResponse
    {
        [JsonPropertyName("data")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TResult Data { get; init; }
    }
}