using System;

namespace TGS.Core.DataService.Models
{
    public class LicenseeCurrency
    {
        public Guid LicenseeId { get; set; }

        public Licensee Licensee { get; set; }

        public string CurrencyCode { get; set; }

        public Currency Currency { get; set; }
    }
}