using System;

namespace TGS.Core.DataServices.Models
{
    public class BrandCurrency
    {
        public Guid Id { get; set; }

        public Guid BrandId { get; set; }

        public Brand Brand { get; set; }

        public string CurrencyCode { get; set; }

        public Currency Currency { get; set; }

        public string BrandCurrencyCode { get; set; }

        public int DisplayOrder { get; set; }
    }
}