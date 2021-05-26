using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BigGaming.Outgoing
{
    public record BigGamingVideoOrderQueryItemResponse
    {
        [JsonPropertyName("orderId")]
        public long OrderId { get; set; }
        [JsonPropertyName("tranId")]
        public long? TranId { get; set; }
        [JsonPropertyName("sn")]
        public string Sn { get; set; }
        [JsonPropertyName("uid")]
        public long Uid { get; set; }
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("loginId")]
        public string LoginId { get; set; }
        [JsonPropertyName("moduleId")]
        public int ModuleId { get; set; }
        [JsonPropertyName("moduleName")]
        public string ModuleName { get; set; }
        [JsonPropertyName("gameId")]
        public int GameId { get; set; }
        [JsonPropertyName("gameName")]
        public string GameName { get; set; }
        [JsonPropertyName("gameNameEn")]
        public string GameNameEn { get; set; }
        [JsonPropertyName("orderStatus")]
        public int OrderStatus { get; set; }
        [JsonPropertyName("bAmount")]
        public decimal BAmount { get; set; }
        [JsonPropertyName("aAmount")]
        public decimal? AAmount { get; set; }
        [JsonPropertyName("orderFrom")]
        public int OrderFrom { get; set; }
        [JsonPropertyName("orderTime")]
        public String OrderTime { get; set; }
        [JsonPropertyName("lastUpdateTime")]
        public String LastUpdateTime { get; set; }
        [JsonPropertyName("fromIp")]
        public string FromIp { get; set; }
        [JsonPropertyName("tableId")]
        public string TableId { get; set; }
        [JsonPropertyName("issueId")]
        public string IssueId { get; set; }
        [JsonPropertyName("playId")]
        public string PlayId { get; set; }
        [JsonPropertyName("playName")]
        public string PlayName { get; set; }
        [JsonPropertyName("playNameEn")]
        public string PlayNameEn { get; set; }
        [JsonPropertyName("validBet")]
        public decimal ValidBet { get; set; }
        [JsonPropertyName("validAmount")]
        public decimal ValidAmount { get; set; }
        [JsonPropertyName("payment")]
        public decimal Payment { get; set; }
        [JsonPropertyName("noComm")]
        public string NoComm { get; set; }

    }
}
