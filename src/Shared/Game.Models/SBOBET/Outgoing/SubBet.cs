using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Outgoing
{
    public record SubBet
    {
        [JsonPropertyName("sportType")]
        public string SportType { get; init; }

        [JsonPropertyName("betOption")]
        public string BetOption { get; init; }

        [JsonPropertyName("marketType")]
        public string MarketType { get; init; }

        [JsonPropertyName("hdp")]
        public decimal HandicapPoint { get; init; }

        [JsonPropertyName("odds")]
        public decimal Odds { get; init; }

        [JsonPropertyName("league")]
        public string League { get; init; }

        [JsonPropertyName("match")]
        public string Match { get; init; }

        [JsonPropertyName("status")]
        public string Status { get; init; }

        [JsonPropertyName("winlostDate")]
        public DateTime? WinLostDate { get; init; }

        [JsonPropertyName("liveScore")]
        public string LiveScore { get; init; }

        [JsonPropertyName("htScore")]
        public string HalfTimeScore { get; init; }

        [JsonPropertyName("ftScore")]
        public string FullTimeScore { get; init; }

        [JsonPropertyName("customeizedBetType")]
        public string CustomizedBetType { get; init; }

        [JsonPropertyName("kickOffTime")]
        public DateTime KickOffTime { get; init; }
    }
}