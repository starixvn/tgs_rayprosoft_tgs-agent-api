using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record AuthRequest : IRequest<AuthResponse>
    {
        [Required]
        [JsonPropertyName("client")]
        public string ApiKey { get; init;}

        [Required]
        [JsonPropertyName("currency")]
        public string Currency { get; init; }
    }
}
