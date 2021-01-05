using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class Licensee : BaseEntity<Guid>
    {
        public Status Status { get; set; }

        public string Description { get; set; }

        public string OperationalEmail { get; set; }

        public Guid ResellerId { get; set; }

        public virtual Reseller Reseller { get; set; }

        public virtual ICollection<Brand> Brands { get; set; }

        public virtual ICollection<LicenseeCurrency> Currencies { get; set; }

        public virtual ICollection<LicenseeLanguage> Languages { get; set; }

        public virtual ICollection<LicenseeBetLimit> BetLimits { get; set; }

        public virtual ICollection<LicenseeGameProvider> GameProviders { get; set; }
        public virtual ICollection<Department> Departments { get; set; }
    }
}