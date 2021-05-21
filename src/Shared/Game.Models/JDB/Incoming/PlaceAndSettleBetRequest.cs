// unset

using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.JDB.Incoming
{
    public record PlaceAndSettleBetRequest : BaseActionRequest
    {
        [JsonPropertyName("transferId")]
        public long TransferId { get; init; }

        [JsonPropertyName("gameSeqNo")]
        public long GameSequenceNo { get; init; }

        [JsonPropertyName("gType")]
        public int GameType { get; init; }

        [JsonPropertyName("mType")]
        public int MachineType { get; init; }

        [JsonPropertyName("reportDate")]
        public string ReportDate { get; init; }

        [JsonPropertyName("gameDate")]
        public string GameDate { get; init; }

        [JsonPropertyName("currency")]
        public string Currency { get; init; }

        [JsonPropertyName("bet")]
        public decimal Bet { get; init; }

        [JsonPropertyName("win")]
        public decimal Win { get; init; }

        [JsonPropertyName("netWin")]
        public decimal NetWin { get; init; }

        [JsonPropertyName("denom")]
        public decimal Denom { get; init; }

        [JsonPropertyName("ipAddress")]
        public string IpAddress { get; init; }

        [JsonPropertyName("clientType")]
        public string ClientType { get; init; }

        [JsonPropertyName("systemTakeWin")]
        public int SystemTakeWin { get; init; }

        [JsonPropertyName("lastModifyTime")]
        public string LastModifyTime { get; init; }

        [JsonPropertyName("mb")]
        public decimal MinBalance { get; init; }

        [JsonPropertyName("jackpotWin")]
        public decimal? JackpotWin { get; init; }

        [JsonPropertyName("jackpotContribute")]
        public decimal? JackpotContribute { get; init; }

        [JsonPropertyName("hasFreegame")]
        public int? HasFreeGame { get; init; }

        [JsonPropertyName("hasGamble")]
        public int? HasGamble { get; init; }

        [JsonPropertyName("roomType")]
        public int? RoomType { get; init; }
    }
}