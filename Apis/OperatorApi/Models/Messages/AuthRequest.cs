using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record AuthRequest : IRequest<AuthResponse>
    {
        [JsonPropertyName("client")]
        public string ApiKey { get; init;}

        [JsonPropertyName("currency")]
        public string Currency { get; init; }
    }
}
