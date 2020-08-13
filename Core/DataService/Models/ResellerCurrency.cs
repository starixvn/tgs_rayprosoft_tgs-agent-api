using System;

namespace TGS.Core.DataServices.Models
{
    public class ResellerCurrency
    {
        public Guid ResellerId { get; set; }

        public Reseller Reseller { get; set; }

        public string CurrencyCode { get; set; }

        public Currency Currency { get; set; }
    }
}