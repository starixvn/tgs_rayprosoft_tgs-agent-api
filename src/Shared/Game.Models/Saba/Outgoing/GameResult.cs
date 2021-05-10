using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record GameResult
    {
        [JsonPropertyName("trans_id")]
        public string ExternalTransactionId { get; init; }

        [JsonPropertyName("vendor_member_id")]
        public string PlayerCode { get; init; }

        [JsonPropertyName("league_id")]
        public int? LeagueId { get; init; }

        [JsonPropertyName("leaguename")]
        public Name[] LeagueName { get; init; }

        [JsonPropertyName("match_id")]
        public int? MatchId { get; init; }

        [JsonPropertyName("home_id")]
        public int? HomeId { get; init; }

        [JsonPropertyName("hometeamname")]
        public Name[] HomeTeamName { get; init; }

        [JsonPropertyName("away_id")]
        public int? AwayId { get; init; }

        [JsonPropertyName("awayteamname")]
        public Name[] AwayTeamName { get; init; }

        [JsonPropertyName("game_id")]
        public string GameId { get; init; }

        [JsonPropertyName("sport_type")]
        public int? SportType { get; init; }

        [JsonPropertyName("bet_type")]
        public int? BetType { get; init; }

        [JsonPropertyName("parlay_ref_no")]
        public int? ParlayReferenceNumber { get; init; }

        [JsonPropertyName("odds")]
        public decimal? Odds { get; init; }

        [JsonPropertyName("stake")]
        public decimal? Stake { get; init; }

        [JsonPropertyName("discount_stake")]
        public decimal? DiscountStake { get; init; }

        [JsonPropertyName("transaction_time")]
        public string TransactionTime { get; init; }

        [JsonPropertyName("ticket_status")]
        public string TicketStatus { get; init; }

        [JsonPropertyName("winlost_amount")]
        public decimal? WinlostAmount { get; init; }

        [JsonPropertyName("after_amount")]
        public decimal? AfterAmount { get; init; }

        [JsonPropertyName("currency")]
        public int CurrencyCode { get; init; }

        [JsonPropertyName("winlost_datetime")]
        public string WinlostDateTime { get; init; }

        [JsonPropertyName("odds_type")]
        public int? OddsType { get; init; }

        [JsonPropertyName("odds_Info")]
        public string OddsInfo { get; init; }

        [JsonPropertyName("isLucky")]
        public bool IsLucky { get; init; }

        [JsonPropertyName("parlay_type")]
        public string ParlayType { get; init; }

        [JsonPropertyName("combo_type")]
        public string ComboType { get; init; }

        [JsonPropertyName("bet_team")]
        public string BetTeam { get; init; }

        [JsonPropertyName("bet_tag")]
        public string BetTag { get; init; }

        [JsonPropertyName("exculding")]
        public string Excluding { get; init; }

        [JsonPropertyName("home_hdp")]
        public decimal? HomeHandicap { get; init; }

        [JsonPropertyName("away_hdp")]
        public decimal? AwayHandicap { get; init; }

        [JsonPropertyName("hdp")]
        public decimal? Handicap { get; init; }

        [JsonPropertyName("ou_hdp")]
        public decimal? OuHandicap { get; init; }

        [JsonPropertyName("betfrom")]
        public string BetFrom { get; init; }

        [JsonPropertyName("islive")]
        public string IsLive { get; init; }

        [JsonPropertyName("home_score")]
        public int? HomeScore { get; init; }

        [JsonPropertyName("away_score")]
        public int? AwayScore { get; init; }

        [JsonPropertyName("settlement_time")]
        public string SettlementTime { get; init; }

        [JsonPropertyName("ba_status")]
        public string BetAggregator { get; init; }

        [JsonPropertyName("percentage")]
        public decimal? Percentage { get; init; }

        [JsonPropertyName("esports_gameid")]
        public int? EsportsGameId { get; init; }

        [JsonPropertyName("total_score")]
        public int? TotalScore { get; init; }

        [JsonPropertyName("resettlementinfo")]
        public ResettlementInfo[] ResettlementInfo { get; init; }

        [JsonPropertyName("match_datetime")]
        public string MatchDateTime { get; init; }

        [JsonPropertyName("ParlayData")]
        public Parlay[] Parlays { get; init; }
    }
}