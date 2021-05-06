using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.Saba.Outgoing
{
    public record Parlay
    {
        [JsonPropertyName("parlay_id")]
        public string ParlayId { get; init; }

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

        [JsonPropertyName("match_datetime")]
        public string MatchDateTime { get; init; }

        [JsonPropertyName("odds")]
        public decimal? Odds { get; init; }

        [JsonPropertyName("bet_type")]
        public int? BetType { get; init; }

        [JsonPropertyName("sport_type")]
        public int? SportType { get; init; }

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

        [JsonPropertyName("bet_team")]
        public string BetTeam { get; init; }

        [JsonPropertyName("bet_tag")]
        public string BetTag { get; init; }

        [JsonPropertyName("ticket_status")]
        public string TicketStatus { get; init; }

        [JsonPropertyName("winlost_amount")]
        public decimal? WinlostAmount { get; init; }

        [JsonPropertyName("after_amount")]
        public decimal? AfterAmount { get; init; }

        [JsonPropertyName("winlost_datetime")]
        public string WinlostDateTime { get; init; }

        [JsonPropertyName("esports_gameid")]
        public int? EsportsGameId { get; init; }
    }
}