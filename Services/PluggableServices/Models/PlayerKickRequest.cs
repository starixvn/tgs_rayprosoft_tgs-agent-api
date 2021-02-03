using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Services.PluggableServices.Models
{
    public record PlayerKickRequest : IRequest<PlayerKickResponse>
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; }
    }
}