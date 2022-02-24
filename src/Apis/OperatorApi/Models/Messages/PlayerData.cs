using System.Text.Json.Serialization;
using TGS.Services.Operator.ApplicationServices.Player;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public record PlayerData
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("identityauthentication")]
        public bool IdentityAuthentication { get; set; }

        [JsonPropertyName("identityformstatus")]
        public PlayerIdentityReviewState IdentityAuthenticationFormStatus { get; set; }

        [JsonPropertyName("autotransfertogame")]
        public bool IsAutoTransferToGame { get; set; }

        [JsonPropertyName("issetpaypassword")]
        public bool IsSetPayPassword { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("playercode")]
        public string PlayerCode { get; set; }

        [JsonPropertyName("qq")]
        public string QQ { get; set; }

        [JsonPropertyName("realname")]
        public string RealName { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("viplevel")]
        public int VipLevel { get; set; }

        [JsonPropertyName("wechat")]
        public string Wechat { get; set; }
    }
}
