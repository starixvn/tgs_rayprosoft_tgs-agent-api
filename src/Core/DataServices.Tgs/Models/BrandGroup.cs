using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class BrandGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid BrandId { get; set; }
        public virtual Brand Brand { get; set; }
        public string Description { get; set; }
        public bool IsSports { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public DateTimeOffset UpdatedOn { get; set; }
        public string CreatedByUsername { get; set; }
        public string UpdatedByUsername { get; set; }
        public virtual ICollection<BrandGroupGame> BrandGames { get; set; }
        public virtual ICollection<BrandGroupBetLimit> BrandGroupBetLimits { get; set; }
    }
}