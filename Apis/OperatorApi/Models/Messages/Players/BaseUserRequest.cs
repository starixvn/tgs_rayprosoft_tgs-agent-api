using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record BaseUserRequest
    {
        [Required]
        [JsonPropertyName("username")]
        public string Username { get; init; }

        [Required]
        [JsonPropertyName("answer")]
        public string Answer { get; init; }

        [Required]
        [JsonPropertyName("captcha")]
        public string Captcha { get; init; }
    }
}