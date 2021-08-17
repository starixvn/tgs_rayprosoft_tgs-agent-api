using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGS.GameProviders.Repositories.Models.ScheduleConfig.WGHorse
{
    public record WGHorseGameProviderScheduleConfig
    {
        public int IntervalDayMax { get; set; }
        public int IntervalDay { get; set; }
        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset EndTime { get; set; }
        public DateTimeOffset LastGameRecordTime { get; set; }
    }
}
