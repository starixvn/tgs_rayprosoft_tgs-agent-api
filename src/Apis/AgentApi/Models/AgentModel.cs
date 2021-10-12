using System;
using System.Text.Json.Serialization;
using TGS.Services.Agent;

namespace TGS.Apis.AgentApi.Models
{
    public class AgentModel
    {
        [JsonPropertyName("agentstatus")]
        public string AgentStatus { get; set; }

        [JsonPropertyName("id")]
        public Guid AgnetId { get; set; }

        [JsonPropertyName("balance")]
        public decimal Balance { get; set; }

        [JsonPropertyName("cardgamerebatepercentage")]
        public decimal CardGameRebatePercentage { get; set; }

        [JsonPropertyName("commissionpercentage")]
        public decimal CommissionPercentage { get; set; }

        /// <summary>
        /// 佔成
        /// </summary>
        [JsonPropertyName("dividendpercentage")]
        public decimal DividendPercentage { get; set; }

        [JsonPropertyName("leveltype")]
        public string LevelType { get; set; }

        [JsonPropertyName("livedealerrebatepercentage")]
        public decimal LiveDealerRebatePercentage { get; set; }

        [JsonPropertyName("lotteryrebatepercentage")]
        public decimal LotteryRebatePercentage { get; set; }

        [JsonPropertyName("registeredon")]
        public DateTimeOffset RegisteredOn { get; set; }

        [JsonPropertyName("slotrebatepercentage")]
        public decimal SlotRebatePercentage { get; set; }

        [JsonPropertyName("sportsrebatepercentage")]
        public decimal SportsRebatePercentage { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("teamnumber")]
        public int TeamNumber { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("virtualsportsrebatepercentage")]
        public decimal VirtualSportsRebatePercentage { get; set; }

        public AgentModel()
        {
        }

        public AgentModel(AgentInfos agentInfos)
        {
            this.AgentStatus = agentInfos.AgentStatus.ToString();
            this.Balance = agentInfos.Balance;
            this.CardGameRebatePercentage = agentInfos.CardGameRebatePercentage;
            this.CommissionPercentage = agentInfos.CommissionPercentage;
            this.DividendPercentage = agentInfos.DividendPercentage;
            this.LevelType = agentInfos.LevelType.ToString();
            this.LiveDealerRebatePercentage = agentInfos.LiveDealerRebatePercentage;
            this.LotteryRebatePercentage = agentInfos.LotteryRebatePercentage;
            this.RegisteredOn = agentInfos.RegisteredOn;
            this.SlotRebatePercentage = agentInfos.SlotRebatePercentage;
            this.SportsRebatePercentage = agentInfos.SportsRebatePercentage;
            this.Status = agentInfos.Status.ToString();
            this.TeamNumber = agentInfos.TeamNumber;
            this.Type = agentInfos.Type.ToString();
            this.Username = agentInfos.Username;
            this.VirtualSportsRebatePercentage = agentInfos.VirtualSportsRebatePercentage;
            this.AgnetId = agentInfos.AgentId;
        }
    }
}
