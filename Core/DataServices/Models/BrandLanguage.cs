using System;

namespace TGS.Core.DataServices.Models
{
    public class BrandLanguage
    {
        public Guid BrandId { get; set; }
        public Brand Brand { get; set; }
        public string CultureCode { get; set; }
        public Language Language { get; set; }
        public string BrandLanguageCode { get; set; }
    }
}