using Tgs.Payment.Clients;

namespace TGS.Services.BackOffice.BrandPayment.PaymentConditions;

internal class DepositedAmountCondition : IBrandPaymentCondition
{
    private ValueRange<decimal> _depositedAmountLimit;

    public DepositedAmountCondition(ValueRange<decimal> depositedAmountLimit)
    {
        _depositedAmountLimit = depositedAmountLimit;
    }

    public bool IsValid(BrandPaymentInfo brandPayment, Tgs.PlayerService.Client.PlayerInfo player, decimal depositedAmount)
        => depositedAmount >= _depositedAmountLimit.From
            && depositedAmount < _depositedAmountLimit.To;
}
