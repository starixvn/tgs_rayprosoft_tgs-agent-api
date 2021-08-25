using System;

namespace TGS.GameProviders.Repositories.Models.ScheduleConfig.WMCasino
{
    public record WMCasinoGameProviderScheduleConfig
    {
        public decimal IntervalMinuteMax { get; set; }
        public decimal IntervalMinute { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset LastGameRecordTime { get; set; }
    }
}