using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.PlayerBucket.Core.Models
{
    public class CreatePlayerBucketRequest
    {
        public Guid BrandId { get; set; }
        public string PlayerCode { get; set; }
        public Guid DefaultWalletId { get; set; }
        public decimal BucketAmount { get; set; }
        public decimal BonusAmount { get; set; }
        public BucketType BucketType { get; set; }
    }
}