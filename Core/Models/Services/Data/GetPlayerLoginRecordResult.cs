using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.Models.Services.Data
{
    public class GetPlayerLoginRecordResult
    {
        public Guid LicenseeId { get; set; }
        public PlatformType PlatformType { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset ExpiredOn { get; set; }
    }
}