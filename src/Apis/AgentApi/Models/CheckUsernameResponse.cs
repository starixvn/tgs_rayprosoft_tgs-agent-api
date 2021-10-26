using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.AgentApi.Models
{
    public record CheckUsernameResponse : TgsResponse
    {
        [JsonPropertyName("isexisted")]
        public bool IsExisted { get; init; }
    }
}