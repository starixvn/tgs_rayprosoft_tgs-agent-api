// ReSharper disable IdentifierTypo

using System;
using System.Text.Json.Serialization;

namespace TGS.Apis.TgsApi.GameModels.SBOBET.Outgoing
{
    public record GetRoundHistoryResponse : BaseResponse
    {
        [JsonPropertyName("Result")]
        public GameResult[] Results { get; init; }
    }

    public record GameResult
    {
        [JsonPropertyName("refNo")]
        public string ReferenceNo { get; init; }

        [JsonPropertyName("username")]
        public string Username { get; init; }

        [JsonPropertyName("sportsType")]
        public string SportsType { get; init; }

        [JsonPropertyName("orderTime")]
        public DateTime OrderTime { get; init; }

        [JsonPropertyName("winLostDate")]
        public DateTime WinLostDate { get; init; }

        [JsonPropertyName("settleTime")]
        public DateTime SettleTime { get; init; }

        [JsonPropertyName("modifyDate")]
        public DateTime ModifyDate { get; init; }

        [JsonPropertyName("odds")]
        public decimal Odds { get; init; }

        [JsonPropertyName("oddsStyle")]
        public string OddsStyle { get; init; }

        [JsonPropertyName("stake")]
        public decimal Stake { get; init; }

        [JsonPropertyName("actualStake")]
        public decimal ActualStake { get; init; }

        [JsonPropertyName("Currency")]
        public string Currency { get; init; }

        [JsonPropertyName("status")]
        public string Status { get; init; }

        [JsonPropertyName("winlost")]
        public decimal WinLose { get; init; }

        [JsonPropertyName("turnover")]
        public decimal Turnover { get; init; }

        [JsonPropertyName("isHalfWonLose")]
        public bool IsHalfWonLose { get; init; }

        [JsonPropertyName("isLive")]
        public bool IsLive { get; init; }

        [JsonPropertyName("maxWinWithoutActualStake")]
        public decimal MaxWinWithoutActualStake { get; init; }

        [JsonPropertyName("ip")]
        public string IpAddress { get; init; }

        [JsonPropertyName("subBet")]
        public SubBet[] SubBets { get; init; }

        #region VirtualSports only

        [JsonPropertyName("gameId")]
        public string GameId { get; init; }

        /// <summary>
        /// Virtual Sports only
        /// </summary>
        [JsonPropertyName("productType")]
        public string ProductType { get; init; }

        #endregion
    }

    public record SubBet
    {
        [JsonPropertyName("sportsType")]
        public string SportsType { get; init; }

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

        [JsonPropertyName("winLostDate")]
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
