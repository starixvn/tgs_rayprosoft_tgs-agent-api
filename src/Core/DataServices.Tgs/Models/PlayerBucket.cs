using System;
using TGS.Core.Models.Enums;

namespace TGS.Core.DataServices.Tgs.Models
{
    public class PlayerBucket
    {
        public Guid Id { get; set; }
        public string PlayerCode { get; set; }
        public decimal BucketAmount { get; set; }
        public BucketType BucketType { get; set; }
        public bool IsFull { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }
}