using System;
using System.Text.Json.Serialization;

namespace TGS.Shared.Game.Models.WMCasino.Outgoing
{
    public record GetDateTimeReportRequest : BaseRequest
    {
        public override string Action => "GetDateTimeReport";

        [JsonIgnore]
        public DateTimeOffset StartTime { get; init; }

        [JsonIgnore]
        public DateTimeOffset? EndTime { get; init; }

        [JsonPropertyName("startTime")]
        public string StartTimeString => StartTime.ToString("yyyyMMddHHmmss");

        [JsonPropertyName("endTime")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string EndTimeString => EndTime?.ToString("yyyyMMddHHmmss");

        [JsonPropertyName("syslang")]
        public string Language => "1"; // 0:中文, 1:英文

        [JsonPropertyName("datatype")]
        public string ReportType => "0"; // 0:输赢报表, 1:小费报表, 2:全部

        [JsonPropertyName("timetype")]
        public string ReportTimeType => "1"; // 0:下注时间, 1:结算时间
    }
}