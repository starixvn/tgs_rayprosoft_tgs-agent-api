using System;

namespace TGS.GameProviders.Repositories.Models.ScheduleConfig.KY
{
    public record KYGameProviderScheduleConfig
    {
        public decimal IntervalMinuteMax { get; set; }
        public decimal IntervalMinute { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset LastGameRecordTime { get; set; }
    }
}
