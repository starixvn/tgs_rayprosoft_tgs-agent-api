using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class LicenseeBetLimit
    {
        public int BetLimitId { get; set; }

        public virtual BetLimit BetLimit { get; set; }

        public Guid LicenseeId { get; set; }

        public virtual Licensee Licensee { get; set; }
    }
}