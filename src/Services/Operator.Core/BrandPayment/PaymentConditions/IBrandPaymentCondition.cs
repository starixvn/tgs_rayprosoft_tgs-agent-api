using TGS.Services.Payment;
using TGS.Services.Operator;

namespace TGS.Services.Operator.BrandPayment.PaymentConditions
{
    public interface IBrandPaymentCondition
    {
        bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player);
    }
}
