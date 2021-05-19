using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    internal class AlwaysFalseChecker : IPromotionChecker
    {
        public ValueTask<bool> CheckAsync(string playerCode)
            => ValueTask.FromResult(false);
    }
}
