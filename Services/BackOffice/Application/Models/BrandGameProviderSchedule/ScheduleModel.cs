using System;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProviderScheduled
{
    public class ScheduleModel
    {
        public Guid Id { get; set; }
        public Guid BrandGameProviderId { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}