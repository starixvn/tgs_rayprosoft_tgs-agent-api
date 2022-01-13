using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerVipLevelUpgradeRecord
    {
        public Guid Id { get; set; }

        public Guid PlayerVipLevelSnapshotId { get; set; }
        public Guid BrandId { get; set; }
        public DateTimeOffset ExecutionOn { get; set; }
        public int Status { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}
