using System;

namespace TGS.Services.Promotion.Services
{
    public record RevertPromotionAmountCommand(Guid PromotionId, decimal Amount);
}
