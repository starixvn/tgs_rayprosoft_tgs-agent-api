using System;

namespace TGS.Core.DataService.Models
{
    public class BrandLanguage
    {
        public Guid Id { get; set; }

        public Guid BrandId { get; set; }

        public Brand Brand { get; set; }

        public string CultureCode { get; set; }

        public Language Language { get; set; }

        public string BrandLanguageCode { get; set; }

        public int DisplayOrder { get; set; }
    }
}