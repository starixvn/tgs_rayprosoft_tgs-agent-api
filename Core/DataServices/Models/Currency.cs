using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class Currency
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string NativeName { get; set; }

        public string Symbol { get; set; }

        public virtual ICollection<ResellerCurrency> ResellerCurrencies { get; set; }

        public virtual ICollection<BrandCurrency> BrandCurrencies { get; set; }

        public virtual ICollection<GameProviderCurrency> GameProviderCurrencies { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        public DateTimeOffset UpdatedOn { get; set; }

        public string CreatedByUsername { get; set; }

        public string UpdatedByUsername { get; set; }
    }
}