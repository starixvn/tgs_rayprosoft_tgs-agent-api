using System;
using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models
{
    public class AgentData
    {
        [JsonPropertyName("accumulatedprofitsharing")]
        public decimal AccumulatedProfitSharing { get; init; }

        [JsonPropertyName("agentdividendtype")]
        public string AgentDividendType { get; init; }

        [JsonPropertyName("agentdomains")]
        public string[] AgentDomains { get; init; }

        [JsonPropertyName("agentlevel")]
        public int AgentLevel { get; init; }

        [JsonPropertyName("balance")]
        public decimal Balance { get; init; }

        [JsonPropertyName("brandUrl")]
        public string BrandUrl { get; init; }

        /// <summary>
        /// 分紅設置
        /// </summary>
        [JsonPropertyName("dividendconditions")]
        public DividendConditionModel[] DividendConditions { get; init; }

        [JsonPropertyName("dividendpercentage")]
        public decimal DividendPercentage { get; init; }

        [JsonPropertyName("email")]
        public string Email { get; init; }

        [JsonPropertyName("id")]
        public Guid Id { get; init; }

        [JsonPropertyName("invitationCode")]
        public string InvitationCode { get; init; }

        [JsonPropertyName("issetpaypassword")]
        public bool IsSetPayPassword { get; init; }

        /// <summary>
        /// 代理種類
        /// </summary>
        [JsonPropertyName("leveltype")]
        public string LevelType { get; init; }

        [JsonPropertyName("parentusername")]
        public string ParentUsername { get; init; }

        [JsonPropertyName("phone")]
        public string Phone { get; init; }

        [JsonPropertyName("qq")]
        public string QQ { get; init; }

        [JsonPropertyName("realname")]
        public string RealName { get; init; }

        [JsonPropertyName("registeredon")]
        public DateTimeOffset RegisteredOn { get; init; }

        [JsonPropertyName("status")]
        public string Status { get; init; }

        /// <summary>
        /// 代理類型
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; init; }

        [JsonPropertyName("username")]
        public string Username { get; init; }

        [JsonPropertyName("wechat")]
        public string Wechat { get; init; }
    }
}
