using System;

namespace TGS.Services.PlayerBucket.Core.Models
{
    public record CreatePlayerBucketWithDepositRequest(Guid BrandId, Guid? PromotionId, string PlayerCode,
        Guid DefaultWalletId,
        decimal DepositAmount);
}