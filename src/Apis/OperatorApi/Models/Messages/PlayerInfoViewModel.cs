using System;
using System.Text.Json.Serialization;
using TGS.Services.Operator.ApplicationServices.Player;

namespace TGS.Apis.OperatorApi.Models.Messages
{
    public class PlayerInfoViewModel
    {
        [JsonPropertyName("birthdate")]
        public DateOnly? Birthdate { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("facebookmessenger")]
        public string FacebookMessenger { get; set; }

        [JsonPropertyName("identityauthentication")]
        public bool IdentityAuthentication { get; set; }

        [JsonPropertyName("identityformid")]
        public Guid? IdentityAuthenticationFormId { get; set; }

        [JsonPropertyName("identityformstatus")]
        public PlayerIdentityReviewState IdentityAuthenticationFormStatus { get; set; }

        [JsonPropertyName("autotransfertogame")]
        public bool IsAutoTransferToGame { get; set; }

        [JsonPropertyName("issetpaypassword")]
        public bool IsSetPayPassword { get; set; }

        [JsonPropertyName("line")]
        public string Line { get; set; }

        [JsonPropertyName("phone")]
        public string Phone { get; set; }

        [JsonPropertyName("areacode")]
        public string PhoneAreaCode { get; set; }

        [JsonPropertyName("playercode")]
        public string PlayerCode { get; set; }

        [JsonPropertyName("qq")]
        public string QQ { get; set; }

        [JsonPropertyName("realname")]
        public string RealName { get; set; }

        [JsonPropertyName("telegram")]
        public string Telegram { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("viber")]
        public string Viber { get; set; }

        [JsonPropertyName("viplevel")]
        public int VipLevel { get; set; }

        [JsonPropertyName("wechat")]
        public string Wechat { get; set; }

        [JsonPropertyName("whatsapp")]
        public string WhatsApp { get; set; }
    }
}
