using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record LogoutRequest : BaseRequest
    {
        [JsonPropertyName("platformtype")]
        public PlatformType PlatformType { get; init; }
    }
}
