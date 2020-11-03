using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Models.Messages.GameProviders
{
    public record GetGameProvidersRequest
    {
        [Required]
        [JsonPropertyName("gametypes")]
        public GameType[] GameTypes { get; init; }

        [Required]
        [JsonPropertyName("platformtype")]
        public PlatformType PlatformType { get; init; }

        [Required]
        [JsonPropertyName("lang")] 
        public string Language { get; init; }

        [Required]
        [JsonPropertyName("currency")]
        public string CurrencyCode { get; init; }

        [JsonPropertyName("includeMaintenance")]
        public bool IncludeMaintenance { get; init; } = false;
    }
}