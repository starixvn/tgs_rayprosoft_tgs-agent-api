using System;
using System.Text.Json.Serialization;
using TGS.Shared.Game.Models.IMESports.Enums;

namespace TGS.Shared.Game.Models.IMESports.Outgoing
{
    public record HistoryResult
    {
        [JsonPropertyName("Provider")]
        public string Provider { get; init; }

        [JsonPropertyName("GameId")]
        public string GameId { get; init; }

        [JsonPropertyName("BetId")]
        public string BetId { get; init; }

        [JsonPropertyName("PlayerId")]
        public string PlayerCode { get; init; }

        [JsonConverter(typeof(StringToNumberConverter))]
        [JsonPropertyName("StakeAmount")]
        public decimal StakeAmount { get; init; }

        [JsonConverter(typeof(StringToNumberConverter))]
        [JsonPropertyName("WinLoss")]
        public decimal WinLoss { get; init; }

        [JsonPropertyName("ProviderPlayerId")]
        public string ProviderPlayerId { get; init; }

        [JsonPropertyName("WagerType")]
        public string WagerType { get; init; }

        [JsonPropertyName("IsSettled")]
        public string StrIsSettle { get; init; }

        [JsonIgnore]
        public bool IsSettled => StrIsSettle == "1";

        [JsonPropertyName("DetailItems")]
        public HistoryDetail[] Details { get; init; }
    }

    public record HistoryDetail
    {
        [JsonPropertyName("Market")]
        public string Market { get; init; }

        [JsonPropertyName("EventName")]
        public string EventName { get; init; }

        [JsonConverter(typeof(StringToDateTimeOffsetConverter))]
        [JsonPropertyName("EventDateTime")]
        public DateTimeOffset EventDateTime { get; init; }

        [JsonPropertyName("CompetitionName")]
        public string CompetitionName { get; init; }

        [JsonPropertyName("HomeTeamName")]
        public string HomeTeamName { get; init; }

        [JsonPropertyName("AwayTeamName")]
        public string AwayTeamName { get; init; }

        [JsonPropertyName("BetType")]
        public string BetType { get; init; }

        [JsonPropertyName("BetTypeDesc")]
        public string BetTypeDesc { get; init; }

        [JsonPropertyName("Period")]
        public string Period { get; init; }

        [JsonPropertyName("Selection")]
        public string BetTypeSelection { get; init; }

        [JsonPropertyName("Odds")]
        public decimal Odds { get; init; }

        [JsonPropertyName("FTScore")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public FullTimeScore[] FullTimeScores { get; init; }

        [JsonPropertyName("Handicap")]
        public string Handicap { get; init; }

        [JsonPropertyName("Sportsname")]
        public string SportsName { get; init; }

        [JsonPropertyName("EventID")]
        public string EventId { get; init; }

        [JsonPropertyName("Specifier")]
        public string Specifier { get; init; }

        [JsonPropertyName("GameOrder")]
        public string GameOrder { get; init; }

        [JsonPropertyName("IsWagerItemCancelled")]
        public IsCancelled IsWagerItemCancelled { get; init; }

        [JsonPropertyName("IsSettled")]
        public IsSettled IsSettled { get; init; }

        [JsonPropertyName("IsResettled")]
        public IsResettled IsResettled { get; init; }

        [JsonPropertyName("ResultStatus")]
        public ResultStatus ResultStatus { get; init; }
    }

    public class FullTimeScore
    {
        [JsonPropertyName("OUScore")]
        public int OUScore { get; init; }

        [JsonPropertyName("Option")]
        public string Option { get; init; }

        [JsonPropertyName("Outcome")]
        public OutcomeType Outcome { get; init; }
    }
}