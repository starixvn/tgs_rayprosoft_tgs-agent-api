using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class BrandPayRecord
    {
        public int Id { get; set; }
        public Guid BrandQuotaId { get; set; }
        public BrandQuota BrandQuota { get; set; }
        public decimal PayAmount { get; set; }
        public decimal Balance { get; set; }
        public string CreatedByUsername { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}