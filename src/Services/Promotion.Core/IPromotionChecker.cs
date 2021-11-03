using System;
using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    public interface IPromotionChecker
    {
        ValueTask<bool> CheckAsync(string playerCode);

        ValueTask CommitTagAsync(string playerCode, Guid? promotionId);

        ValueTask RollbackTagAsync(string playerCode, Guid? promotionId);

        ValueTask TagPromotionTypeAsync(string playerCode, Guid? promotionId);
    }
}
