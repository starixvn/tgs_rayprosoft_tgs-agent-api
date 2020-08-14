using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class Reseller : BaseEntity
    {
        public string Description { get; set; }

        public string OperationalEmail { get; set; }

        public virtual ICollection<Licensee> Licensees { get; set; }

        public virtual ICollection<Brand> Brands { get; set; }

        public virtual ICollection<ResellerGameProvider> GameProviders { get; set; }

        public virtual ICollection<ResellerLanguage> Languages { get; set; }

        public virtual ICollection<ResellerCurrency> Currencies { get; set; }

    }
}