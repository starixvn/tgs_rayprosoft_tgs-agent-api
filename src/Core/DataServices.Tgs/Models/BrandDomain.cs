using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class BrandDomain
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public Guid BrandId { get; set; }
        public virtual Brand Brand { get; set; }
    }
}