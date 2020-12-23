using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record GetCaptchaResponse : TgsResponse
    {
        [JsonPropertyName("image")]
        public byte[] Image { get; init; }

        [JsonPropertyName("captcha")]
        public string Captcha { get; set; }

        [JsonPropertyName("contenttype")]
        public string ContentType { get; set; }
    }
}
