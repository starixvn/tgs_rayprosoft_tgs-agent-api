using System;

namespace TGS.Core.DataServices.Models
{
    public class BrandLanguage
    {
        public Guid BrandId { get; set; }

        public virtual Brand Brand { get; set; }

        public string CultureCode { get; set; }

        public virtual Language Language { get; set; }

        public string BrandCultureCode { get; set; }
    }
}