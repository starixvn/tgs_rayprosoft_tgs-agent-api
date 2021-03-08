using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.SBOBET.Outgoing
{
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

        [JsonPropertyName("currency")]
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
        ///     Virtual Sports only
        /// </summary>
        [JsonPropertyName("productType")]
        public string ProductType { get; init; }

        #endregion
    }
}