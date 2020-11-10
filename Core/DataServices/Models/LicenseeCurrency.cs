using System;

namespace TGS.Core.DataServices.Models
{
    public class LicenseeCurrency
    {
        public Guid LicenseeId { get; set; }

        public virtual Licensee Licensee { get; set; }

        public string CurrencyCode { get; set; }

        public virtual Currency Currency { get; set; }

        public string LicenseeCurrencyCode { get; set; }
    }
}