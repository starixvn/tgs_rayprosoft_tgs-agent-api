using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class BrandGameProviderSchedule
    {
        public Guid Id { get; set; }
        public Guid BrandGameProviderId { get; set; }
        public virtual BrandGameProvider BrandGameProvider { get; set; }
        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }
        public Status Status { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public bool IsDefault { get; set; }
    }
}