using System;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule
{
    public class ScheduleRequest : BaseRequest
    {
        public Guid Id { get; set; }
        public Guid GameProviderId { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
    }
}