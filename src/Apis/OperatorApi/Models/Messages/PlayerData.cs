using System.Text.Json.Serialization;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record PlayerData
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("realname")]
        public string RealName { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("qq")]
        public string QQ { get; set; }

        [JsonPropertyName("wechat")]
        public string Wechat { get; set; }

        [JsonPropertyName("issetpaypassword")]
        public bool IsSetPayPassword { get; set; }

        [JsonPropertyName("identityauthentication")]
        public bool IdentityAuthentication { get; set; }
    }
}