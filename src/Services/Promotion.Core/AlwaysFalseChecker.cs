using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    internal class AlwaysFalseChecker : IPromotionChecker
    {
        public ValueTask<bool> CheckAsync(string playerCode)
            => ValueTask.FromResult(false);

        public ValueTask CommitTagAsync(string playerCode) => ValueTask.CompletedTask;

        public ValueTask RollbackTagAsync(string playerCode) => ValueTask.CompletedTask;

        public ValueTask TagPromotionTypeAsync(string playerCode) => ValueTask.CompletedTask;
    }
}
