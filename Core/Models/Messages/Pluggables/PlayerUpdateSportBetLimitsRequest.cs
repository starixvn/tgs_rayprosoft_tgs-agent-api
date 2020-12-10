using System.Text.Json.Serialization;

namespace TGS.Core.Models.Messages.Pluggables
{
    public record PlayerUpdateSportBetLimitsRequest
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [JsonPropertyName("playercode")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("betlimits")]
        public SportBetLimits[] BetLimits { get; init; }
    }

    public record SportBetLimits
    {
        [JsonPropertyName("sporttype")]
        public string SportType { get; init; }

        [JsonPropertyName("markettype")]
        public string MarketType { get; init; }

        [JsonPropertyName("minbet")]
        public int MinBet { get; init; }

        [JsonPropertyName("maxbet")]
        public int MaxBet { get; init; }

        [JsonPropertyName("maxbetpermatch")]
        public int MaxBetPerMatch { get; init; }
    }
}