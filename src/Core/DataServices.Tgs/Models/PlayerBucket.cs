using System;
using System.Collections.Generic;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerBucket
    {
        public Guid Id { get; set; }
        public decimal BucketAmount { get; set; }
        public decimal TurnoverAmount { get; set; }
        public BucketType BucketType { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
        public virtual ICollection<PlayerBucketTurnoverRecord> PlayerBucketTurnoverRecord { get; set; }
    }
}