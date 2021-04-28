using System;
using System.Collections.Generic;

namespace TGS.Core.Models.Bonus
{
    public record PromotionPlayerBonus(IEnumerable<PlayerBonus> PlayerBonus, Guid PromotionId, Guid PromotionBonusId,
        string PromotionType);
}