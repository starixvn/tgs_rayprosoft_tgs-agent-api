using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Licensee : BaseEntity
    {
        public string Description { get; set; }

        public Status Status { get; set; }

        public virtual ICollection<Brand> Brands { get; set; }

        public virtual ICollection<LicenseeLanguage> Languages { get; set; }

        public virtual ICollection<LicenseeCurrency> Currencies { get; set; }

        public Guid ResellerId { get; set; }

        public Reseller Reseller { get; set; }
    }
}