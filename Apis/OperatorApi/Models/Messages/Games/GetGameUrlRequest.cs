using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Models.Messages.Games
{
    public class GetGameUrlRequest
    {
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; init; }

        [Required]
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [Required]
        [JsonPropertyName("gcode")]
        public string GameCode { get; init; }

        [Required]
        [JsonPropertyName("platformtype")]
        public PlatformType PlatformType { get; init; }
    }
}