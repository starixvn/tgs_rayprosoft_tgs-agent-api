using System;

namespace TGS.Core.DataServices.Models
{
    public class LicenseeBetLimit
    {
        public int BetLimitGroupId { get; set; }

        public BetLimitGroup BetLimitGroup { get; set; }

        public Guid LicenseeId { get; set; }

        public Licensee Licensee { get; set; }
    }
}