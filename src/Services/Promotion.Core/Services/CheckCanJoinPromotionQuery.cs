using System;

namespace TGS.Services.Promotion.Services
{
    public record CheckCanJoinPromotionQuery(
        Guid BrandId,
        string PlayerCode,
        Guid PromotionId,
        decimal Amount);
}