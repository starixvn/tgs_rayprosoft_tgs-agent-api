using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule
{
    public class ScheduleModel
    {
        public Guid Id { get; set; }
        public Status Status { get; set; }
        public Guid BrandGameProviderId { get; set; }
        public string GameProviderName { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
    }
}