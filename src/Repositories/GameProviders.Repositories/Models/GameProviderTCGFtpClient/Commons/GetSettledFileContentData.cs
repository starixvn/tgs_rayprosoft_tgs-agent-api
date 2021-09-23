using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TGS.GameProviders.Repositories.Models.GameProviderTCGFtpClient.Commons
{
    public record GetSettledFileContentData
    {
        [JsonPropertyName("betAmount")]
        public decimal BetAmount { get; set; }
        [JsonPropertyName("gameCode")]
        public string GameCode { get; set; }
        [JsonPropertyName("betOrderNo")]
        public string BetOrderNo { get; set; }
        [JsonPropertyName("betTime")]
        public string BetTime { get; set; }
        [JsonPropertyName("transTime")]
        public string TransTime { get; set; }
        [JsonPropertyName("betContentId")]
        public string BetContentId { get; set; }
        [JsonPropertyName("playCode")]
        public string PlayCode { get; set; }
        [JsonPropertyName("orderNum")]
        public string OrderNum { get; set; }
        [JsonPropertyName("chase")]
        public string Chase { get; set; }
        [JsonPropertyName("numero")]
        public string Numero { get; set; }
        [JsonPropertyName("merchantCode")]
        public string MerchantCode { get; set; }
        [JsonPropertyName("bettingContent")]
        public string BettingContent { get; set; }
        [JsonPropertyName("playId")]
        public int PlayId { get; set; }
        [JsonPropertyName("freezeTime")]
        public string FreezeTime { get; set; }
        [JsonPropertyName("multiple")]
        public int Multiple { get; set; }
        [JsonPropertyName("remark")]
        public string Remark { get; set; }
        [JsonPropertyName("playName")]
        public string PlayName { get; set; }
        [JsonPropertyName("gameGroupName")]
        public string GameGroupName { get; set; }
        [JsonPropertyName("device")]
        public string Device { get; set; }
        [JsonPropertyName("winAmount")]
        public decimal WinAmount { get; set; }
        [JsonPropertyName("netPNL")]
        public decimal NetPNL { get; set; }
        [JsonPropertyName("betStatus")]
        public string BetStatus { get; set; }
        [JsonPropertyName("settlementTime")]
        public string SettlementTime { get; set; }
        [JsonPropertyName("actualBetAmount")]
        public string ActualBetAmount { get; set; }
        [JsonPropertyName("exceedWinAmount")]
        public decimal ExceedWinAmount { get; set; }
        [JsonPropertyName("details")]
        public string Details { get; set; }
        [JsonPropertyName("winningNumber")]
        public string WinningNumber { get; set; }
        [JsonPropertyName("clientIp")]
        public string ClientIp { get; set; }
        [JsonPropertyName("playBonus")]
        public decimal PlayBonus { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
        [JsonPropertyName("productType")]
        public string ProductType { get; set; }
        [JsonPropertyName("single")]
        public bool Single { get; set; }
        [JsonPropertyName("detailStatusId")]
        public int DetailStatusId { get; set; }
    }
}
