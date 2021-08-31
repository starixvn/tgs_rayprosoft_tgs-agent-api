using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Repositories.Models.ScheduleConfig.WGSports
{
    public record WGSportsGameProviderScheduleConfig
    {
        public decimal IntervalMinuteMax { get; set; }
        public decimal IntervalMinute { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset LastGameRecordTime { get; set; }
    }
}
