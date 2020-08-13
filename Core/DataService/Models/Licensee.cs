using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class Licensee : BaseEntity
    {
        public string Description { get; set; }

        public string OperationalEmail { get; set; }

        public int BrandLimit { get; set; }

        public virtual ICollection<Brand> Brands { get; set; }

        public virtual ICollection<LicenseeLanguage> Languages { get; set; }

        public virtual ICollection<LicenseeCurrency> Currencies { get; set; }

        public Guid ResellerId { get; set; }

        public Reseller Reseller { get; set; }
    }
}