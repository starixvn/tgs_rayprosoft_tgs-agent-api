// unset

using System.Text.Json.Serialization;
using TGS.Core.Models.Enums;
using JDBGameType = TGS.Shared.Game.Models.JDB.Enums.GameType;

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record PlaceAndSettleBetRequest : BaseActionRequest
    {
        [JsonPropertyName("transferId")]
        public long TransferId { get; set; }
        [JsonPropertyName("gameSeqNo")]
        public long GameSequenceNo { get; set; }
        [JsonPropertyName("gType")]
        public JDBGameType GameType { get; set; }
        [JsonPropertyName("mType")]
        public int MachineType { get; set; }
        [JsonPropertyName("reportDate")]
        public string ReportDate { get; set; }
        [JsonPropertyName("gameDate")]
        public string GameDate { get; set; }
        [JsonPropertyName("currency")]
        public string Currency { get; set; }
        [JsonPropertyName("bet")]
        public decimal Bet { get; set; }
        [JsonPropertyName("win")]
        public decimal Win { get; set; }
        [JsonPropertyName("netWin")]
        public decimal NetWin { get; set; }
        [JsonPropertyName("denom")]
        public decimal Denom { get; set; }
        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; set; }
        [JsonPropertyName("clientType")]
        public string ClientType { get; set; }
        [JsonPropertyName("systemTakeWin")]
        public int SystemTakeWin { get; set; }
        [JsonPropertyName("lastModifyTime")]
        public string LastModifyTime { get; set; }
        [JsonPropertyName("mb")]
        public decimal MinBalance { get; set; }
        [JsonPropertyName("jackpotWin")]
        public decimal? JackpotWin { get; set; }
        [JsonPropertyName("jackpotContribute")]
        public decimal? JackpotContribute { get; set; }
        [JsonPropertyName("hasFreegame")]
        public int? HasFreeGame { get; set; }
        [JsonPropertyName("hasGamble")]
        public int? HasGamble { get; set; }
        [JsonPropertyName("roomType")]
        public int? RoomType { get; set; }
    }
}