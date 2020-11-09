using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record BaseTokenRequest
    {
        [Required]
        [JsonPropertyName("token")]
        public string PlayerToken { get; init; }
    }
}