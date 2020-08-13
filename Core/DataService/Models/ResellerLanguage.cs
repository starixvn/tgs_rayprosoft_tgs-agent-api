using System;

namespace TGS.Core.DataService.Models
{
    public class ResellerLanguage
    {
        public Guid ResellerId { get; set; }

        public Reseller Reseller { get; set; }

        public string CultureCode { get; set; }

        public Language Language { get; set; }
    }
}