using System;
using System.Text.Json.Serialization;

namespace TGS.Core.Models.Messages.Pluggables
{
    public record PlayerUpdateSportBetLimitsRequest
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("sportmarketids")]
        public Guid[] SportMarketTypeIds { get; init; }
    }
}