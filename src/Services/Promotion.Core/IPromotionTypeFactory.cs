using System.Threading.Tasks;

namespace TGS.Services.Promotion
{
    public interface IPromotionTypeFactory
    {
        ValueTask<bool> IsDepositTypeAsync(string promotionType);
    }
}