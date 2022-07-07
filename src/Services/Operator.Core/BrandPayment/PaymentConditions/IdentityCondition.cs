using Tgs.Payment.Clients;

namespace TGS.Services.Operator.BrandPayment.PaymentConditions;

internal class IdentityCondition : IBrandPaymentCondition
{
    private readonly bool _enabled;

    public IdentityCondition(bool enabled)
    {
        _enabled = enabled;
    }

    public bool IsValid(BrandPaymentInfo brandPayment, Tgs.PlayerService.Client.PlayerInfo player, decimal depositedAmount)
        => _enabled ? player.IdentityAuthentication : true;
}
