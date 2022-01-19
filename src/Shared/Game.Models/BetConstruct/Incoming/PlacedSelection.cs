using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.BetConstruct.Incoming
{
    public class PlacedSelection
    {
        /// <summary>
        /// Unique ID of the team/player.
        /// </summary>
        [JsonPropertyName("AwayTeamId")]
        public int? AwayTeamId { get; init; }

        /// <summary>
        /// The name of the team/player.
        /// </summary>
        [JsonPropertyName("AwayTeamName")]
        public string AwayTeamName { get; init; }

        /// <summary>
        /// Handicap value for handicap type markets. For  example, -1, 2.5, etc.
        /// </summary>
        [JsonPropertyName("Basis")]
        public decimal Basis { get; init; }

        /// <summary>
        /// Unique ID of the competition of the match in  BetConstructBE. (Mandatory)
        /// </summary>
        [JsonPropertyName("CompetitionId")]
        public int CompetitionId { get; init; }

        /// <summary>
        /// The name of the competition. For example, La Liga.
        /// </summary>
        [JsonPropertyName("CompetitionName")]
        public string CompetitionName { get; init; }

        /// <summary>
        /// UTC time of the end of the match.
        /// </summary>
        [JsonPropertyName("EventEndDate")]
        public DateTimeOffset EventEndDate { get; init; }

        /// <summary>
        /// Unique ID of the team/player.
        /// </summary>
        [JsonPropertyName("HomeTeamId")]
        public int HomeTeamId { get; init; }

        /// <summary>
        /// The name of the team/player.
        /// </summary>
        [JsonPropertyName("HomeTeamName")]
        public string HomeTeamName { get; init; }

        /// <summary>
        /// True – when the selection is on live match, False – Pre Match.
        /// </summary>
        [JsonPropertyName("IsLive")]
        public bool IsLive { get; init; }

        /// <summary>
        /// True - when the selection is an outright match/market,  otherwise - False.
        /// </summary>
        [JsonPropertyName("IsOutright")]
        public bool IsOutright { get; init; }

        /// <summary>
        /// The name of the market. For example, Match Result.
        /// </summary>
        [JsonPropertyName("MarketName")]
        public string MarketName { get; init; }

        /// <summary>
        /// Unique ID of the selection group (market) in  BetConstructBE to identify market type. (Mandatory)
        /// </summary>
        [JsonPropertyName("MarketTypeId")]
        public long MarketTypeId { get; init; }

        /// <summary>
        /// Unique ID of the match in BetConstructBE.
        /// (Mandatory)
        /// </summary>
        [JsonPropertyName("MatchId")]
        public int MatchId { get; init; }

        /// <summary>
        /// A text describing the score and some live info when the  bet is placed on this Live event.
        /// </summary>
        [JsonPropertyName("MatchInfo")]
        public string MatchInfo { get; init; }

        /// <summary>
        /// The name of the match. For example, Barcelona – Real  Madrid.
        /// </summary>
        [JsonPropertyName("MatchName")]
        public string MatchName { get; init; }

        /// <summary>
        /// UTC time of the start of the match.
        /// </summary>
        [JsonPropertyName("MatchStartDate")]
        public DateTimeOffset MatchStartDate { get; init; }

        /// <summary>
        /// The price/odd of the selection. (Mandatory)
        /// </summary>
        [JsonPropertyName("Price")]
        public decimal Price { get; init; }

        /// <summary>
        /// Unique ID of the region the match is taking place in  BetConstructBE. (Mandatory)
        /// </summary>
        [JsonPropertyName("RegionId")]
        public int RegionId { get; init; }

        /// <summary>
        /// The name of the region. For example, Spain.
        /// </summary>
        [JsonPropertyName("RegionName")]
        public string RegionName { get; init; }

        /// <summary>
        /// Unique ID of the bet’s selection/event in
        /// BetConstructBE to identify the selection. (Mandatory)
        /// </summary>
        [JsonPropertyName("SelectionId")]
        public long SelectionId { get; init; }

        /// <summary>
        /// The name of the selection/event. For example, P1.
        /// </summary>
        [JsonPropertyName("SelectionName")]
        public string SelectionName { get; init; }

        /// <summary>
        /// The full name of the sport, It’s also translatable.
        /// </summary>
        [JsonPropertyName("SportFullName")]
        public string SportFullName { get; init; }

        /// <summary>
        /// Unique ID of the sport in BetConstructBE.
        /// (Mandatory)
        /// </summary>
        [JsonPropertyName("SportId")]
        public int SportId { get; init; }

        /// <summary>
        /// The name of the sport. For example, Football.
        /// </summary>
        [JsonPropertyName("SportName")]
        public string SportName { get; init; }
    }
}
