using System;
using System.Text.Json.Serialization;
using MediatR;

namespace TGS.Services.PluggableServices.Models
{
    public record PlayerUpdateSportBetLimitsRequest : IRequest<PlayerUpdateSportBetLimitsResponse>
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("sportmarketids")]
        public Guid[] SportMarketTypeIds { get; init; }
    }
}