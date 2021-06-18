using System.Text.Json.Serialization;
using TGS.Core.Models.Messages;

namespace TGS.Apis.AgentApi.Models
{
    public record GetAgentResponse : TgsResponse
    {
        [JsonPropertyName("data")]
        public AgentData Data { get; init; }
    }

    public record AgentData
    {
        [JsonPropertyName("username")]
        public string Username { get; init; }

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

        [JsonPropertyName("invitationCode")]
        public string InvitationCode { get; init; }

        [JsonPropertyName("brandUrl")]
        public string BrandUrl { get; init; }

        [JsonPropertyName("issetpaypassword")]
        public bool IsSetPayPassword { get; init; }
    }
}