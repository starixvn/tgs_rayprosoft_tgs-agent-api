using System;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule
{
    public class DefaultTimeModel
    {
        public Guid Id { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public bool IsExpired { get; set; }
    }
}