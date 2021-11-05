using System;
using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    internal class AlwaysTrueChecker : IPromotionChecker
    {
        public ValueTask<bool> CheckAsync(string playerCode)
            => ValueTask.FromResult(true);

        public ValueTask CommitTagAsync(string playerCode, Guid? promotionId) => ValueTask.CompletedTask;

        public ValueTask RollbackTagAsync(string playerCode, Guid? promotionId) => ValueTask.CompletedTask;

        public ValueTask TagPromotionTypeAsync(string playerCode, Guid? promotionId) => ValueTask.CompletedTask;
    }
}
