using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;
using TGS.Core.Models.Enums;

namespace TGS.Apis.AgentApi.Models
{
    public record LoginRequest : BaseUserRequest, IRequest<LoginResponse>
    {
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; init; }

        [Required]
        [JsonPropertyName("platformtype")]
        public PlatformType PlatformType { get; init; }
    }
}