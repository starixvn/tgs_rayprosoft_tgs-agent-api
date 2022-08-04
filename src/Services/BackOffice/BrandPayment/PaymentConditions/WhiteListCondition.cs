using System;
using System.Collections.Generic;
using System.Linq;
using Tgs.Payment.Clients;

namespace TGS.Services.BackOffice.BrandPayment.PaymentConditions;

internal class WhiteListCondition : IBrandPaymentCondition
{
    private readonly Guid[] _whiteList;

    public WhiteListCondition(IEnumerable<Guid> playerAllowBrandPaymentIds)
    {
        _whiteList = playerAllowBrandPaymentIds.ToArray();
    }

    public bool IsValid(BrandPaymentInfo brandPayment, Tgs.PlayerService.Client.PlayerInfo player, decimal depositedAmount)
        => _whiteList.Contains(brandPayment.Id);
}
