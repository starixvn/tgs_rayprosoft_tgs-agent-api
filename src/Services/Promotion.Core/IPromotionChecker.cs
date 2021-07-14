using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    public interface IPromotionChecker
    {
        ValueTask<bool> CheckAsync(string playerCode);

        ValueTask CommitTagAsync(string playerCode);

        ValueTask RollbackTagAsync(string playerCode);

        ValueTask TagPromotionTypeAsync(string playerCode);
    }
}
