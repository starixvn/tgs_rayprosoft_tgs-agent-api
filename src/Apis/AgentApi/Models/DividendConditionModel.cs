using System.Text.Json.Serialization;

namespace TGS.Apis.AgentApi.Models
{
    public class DividendConditionModel
    {
        /// <summary>
        /// 活躍人數
        /// </summary>
        [JsonPropertyName("activenumber")]
        public int ActiveNumber { get; set; }

        /// <summary>
        /// 分紅比例
        /// </summary>
        [JsonPropertyName("dividendpercentage")]
        public decimal DividendPercentage { get; set; }

        /// <summary>
        /// 淨營利
        /// </summary>
        [JsonPropertyName("netprofit")]
        public decimal NetProfit { get; set; }

        [JsonPropertyName("rank")]
        public int Rank { get; set; }
    }
}
