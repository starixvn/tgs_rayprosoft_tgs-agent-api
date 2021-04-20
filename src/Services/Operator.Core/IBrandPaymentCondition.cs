using System.Threading.Tasks;
using TGS.Services.Payment;

namespace TGS.Services.Operator
{
    public interface IBrandPaymentCondition
    {
        bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player);
    }
}
