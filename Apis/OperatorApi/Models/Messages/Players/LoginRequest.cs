using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record LoginRequest : BaseRequest
    {
        [Required]
        [JsonPropertyName("platformtype")]
        public PlatformType PlatformType { get; init; }

        [Required]
        [JsonPropertyName("password")]
        public string Password { get; init; }
    }
}
