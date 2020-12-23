using System;
using System.Collections.Generic;

namespace TGS.Core.DataServices.Models
{
    public class BrandGroup : BaseEntity<Guid>
    {
        public Guid BrandId { get; set; }

        public virtual Brand Brand { get; set; }

        public string Description { get; set; }

        public bool IsSports { get; set; }

        public virtual ICollection<BrandGroupGame> BrandGames { get; set; }

        public virtual ICollection<BrandGroupBetLimit> BrandGroupBetLimits { get; set; }
    }
}