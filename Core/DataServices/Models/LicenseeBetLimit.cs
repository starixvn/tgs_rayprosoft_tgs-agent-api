using System;

namespace TGS.Core.DataServices.Models
{
    public class LicenseeBetLimit
    {
        public int BetLimitGroupId { get; set; }

        public virtual BetLimitGroup BetLimitGroup { get; set; }

        public Guid LicenseeId { get; set; }

        public virtual Licensee Licensee { get; set; }
    }
}