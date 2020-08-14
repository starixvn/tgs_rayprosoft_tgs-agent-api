using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Models
{
    public class Reseller : BaseEntity
    {
        public string Description { get; set; }
        
        public Status Status { get; set; }
        
        public string OperationalEmail { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public int? LicenseeLimit { get; set; }

        public int? BrandLimit { get; set; }

        public virtual ICollection<Licensee> Licensees { get; set; }

        public virtual ICollection<Brand> Brands { get; set; }

        public virtual ICollection<ResellerGameProvider> GameProviders { get; set; }

        public virtual ICollection<ResellerLanguage> Languages { get; set; }

        public virtual ICollection<ResellerCurrency> Currencies { get; set; }
    }
}