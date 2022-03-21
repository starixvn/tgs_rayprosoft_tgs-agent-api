using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.BackOffice.Application.Models.PlayerBucket
{
    public record PlayerBucketModel
    {
        public Guid Id { get; set; }

        public string PlayerCode { get; set; }

        public decimal BucketAmount { get; set; }

        public decimal CurrentBucketAmount { get; set; }

        public BucketType BucketType { get; set; }

        public DateTimeOffset CreatedOn { get; set; }
    }
}