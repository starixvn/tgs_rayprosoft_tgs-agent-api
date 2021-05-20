using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    public interface IPromotionChecker
    {
        ValueTask<bool> CheckAsync(string playerCode);
    }
}
