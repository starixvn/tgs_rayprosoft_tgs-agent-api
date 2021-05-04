using System;
using TGS.Core.Models.Enums;

namespace TGS.Services.PlayerBucket.Core.Models
{
    public record CreatePlayerBucketRequest(Guid BrandId, string PlayerCode, Guid DefaultWalletId,
        BucketType BucketType,
        decimal BucketAmount);
}