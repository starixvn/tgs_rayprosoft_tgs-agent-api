using Tgs.Payment.Clients;
using Tgs.PlayerService.Client;

namespace TGS.Services.BackOffice.BrandPayment.PaymentConditions;

public interface IBrandPaymentCondition
{
    bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player, decimal depositedAmount);
}
