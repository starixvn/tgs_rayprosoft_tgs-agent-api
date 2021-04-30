// unset

using System.Text.Json.Serialization;
using JDBGameType = TGS.Shared.Game.Models.JDB.Enums.GameType;

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record PlaceAndSettleBetRequest : BaseActionRequest
    {
        public long TransferId { get; set; }
        public long GameSeqNo { get; set; }
        [JsonPropertyName("gType")]
        public JDBGameType GameType { get; set; }
        [JsonPropertyName("mType")]
        public int MachineType { get; set; }
        public string ReportDate { get; set; }
        public string GameDate { get; set; }
        public string Currency { get; set; }
        public decimal Bet { get; set; }
        public decimal Win { get; set; }
        public decimal NetWin { get; set; }
        public decimal Denom { get; set; }
        public string IpAddress { get; set; }
        public string ClientType { get; set; }
        public int SystemTakeWin { get; set; }
        public string LastModifyTime { get; set; }
        public decimal MinBalance { get; set; }
        public decimal JackpotWin { get; set; }
        public decimal JackpotContribute { get; set; }
        public int HasFreeGame { get; set; }
        public int HasGamble { get; set; }
        public int RoomType { get; set; }

    }
}