using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record LoginRequest : BaseUserRequest
    {
        [JsonPropertyName("password")]
        public string Password { get; init; }

        [JsonPropertyName("platformtype")]
        public PlatformType PlatformType { get; init; }
    }
}
