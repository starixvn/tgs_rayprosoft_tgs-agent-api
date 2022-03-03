using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class BrandGameProviderAggregateAuditLog
    {
        public int Id { get; set; }

        public Guid? BrandId { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}