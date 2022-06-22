using Tgs.Payment.Clients;

namespace TGS.Services.Operator.BrandPayment.PaymentConditions;

public interface IBrandPaymentCondition
{
    bool IsValid(BrandPaymentInfo brandPayment, Tgs.PlayerService.Client.PlayerInfo player, decimal depositedAmount);
}
