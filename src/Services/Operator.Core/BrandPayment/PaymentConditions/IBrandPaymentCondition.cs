using Tgs.Payment.Clients;

namespace TGS.Services.Operator.BrandPayment.PaymentConditions
{
    public interface IBrandPaymentCondition
    {
        bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player);
    }
}
