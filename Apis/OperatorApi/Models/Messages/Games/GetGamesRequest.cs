using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Models.Messages.Games
{
    public record GetGamesRequest
    {
        [Required]
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [Required]
        [JsonPropertyName("platformtype")]
        public PlatformType PlatformType { get; init; }

        [Required]
        [JsonPropertyName("language")]
        public string Language { get; init; }
    }
}
