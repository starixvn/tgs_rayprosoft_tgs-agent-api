using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Games
{
    public class GetGameUrlRequest
    {
        [Required]
        [JsonPropertyName("gpcode")]
        public string GameProviderCode { get; init; }

        [Required]
        [JsonPropertyName("gcode")]
        public string GameCode { get; init; }
    }
}