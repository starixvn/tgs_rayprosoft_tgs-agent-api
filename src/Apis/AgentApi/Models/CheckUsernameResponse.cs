using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.AgentApi.Models
{
    public record CheckUsernameResponse : TgsResponse
    {
        [JsonPropertyName("isexist")]
        public bool Exists { get; init; }
    }
}
