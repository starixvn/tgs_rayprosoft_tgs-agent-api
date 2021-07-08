using System;

namespace TGS.Services.Promotion.Services
{
    public record GetValidPromotionsByPlayerQuery(
        Guid BrandId,
        string PlayerCode);
}
