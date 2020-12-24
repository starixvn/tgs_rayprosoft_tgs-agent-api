using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetGamesRequest
    {
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [JsonPropertyName("platformtype")]
        public PlatformType PlatformType { get; init; }

        [JsonPropertyName("language")]
        public string LanguageCode { get; init; }

        [JsonPropertyName("currency")]
        public string CurrencyCode { get; init; }
    }
}
