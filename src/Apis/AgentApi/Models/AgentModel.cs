using System;
using System.Text.Json.Serialization;

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
    }
}
