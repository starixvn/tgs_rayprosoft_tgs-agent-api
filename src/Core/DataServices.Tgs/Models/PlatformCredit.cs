using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlatformCredit
    {
        public Guid Id { get; set; }
        public Guid BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public virtual ICollection<PlatformPayRecord> BrandPayRecords { get; set; }
    }
}