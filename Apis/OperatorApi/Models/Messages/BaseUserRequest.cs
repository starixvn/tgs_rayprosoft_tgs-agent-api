using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
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

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("bypasscaptcha")]
        public bool? BypassCaptcha { get; init; }
    }
}