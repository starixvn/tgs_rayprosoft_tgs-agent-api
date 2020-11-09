using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages.Players
{
    public record UpdatePlayerRequest : BaseTokenRequest
    {
        [JsonPropertyName("realname")]
        public string RealName { get; init; }

        [JsonPropertyName("phone")]
        public string Phone { get; init; }

        [JsonPropertyName("email")]
        public string Email { get; init; }

        [JsonPropertyName("qq")]
        public string QQ { get; init; }

        [JsonPropertyName("wechat")]
        public string Wechat { get; init; }
    }
}
