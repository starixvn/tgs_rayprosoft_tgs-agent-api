using System;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerBucketTurnoverRecord
    {
        public int Id { get; set; }
        public decimal Turnover { get; set; }
        public Guid PlayerBucketId { get; set; }
        public Guid TransactionId { get; set; }
        public virtual Transaction Transaction { get; set; }
        public virtual PlayerBucket PlayerBucket { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}