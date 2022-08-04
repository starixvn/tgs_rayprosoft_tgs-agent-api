using System;
using System.Collections.Generic;
using System.Linq;
using Tgs.Payment.Clients;
namespace TGS.Services.BackOffice.BrandPayment.PaymentConditions;

internal class BlockListCondition : IBrandPaymentCondition
{
    private Guid[] _blockList;

    public BlockListCondition(IEnumerable<Guid> playerBlockedBrandPaymentIds)
    {
        _blockList = playerBlockedBrandPaymentIds.ToArray();
    }

    public bool IsValid(BrandPaymentInfo brandPayment, Tgs.PlayerService.Client.PlayerInfo player, decimal depositedAmount)
        => !_blockList.Contains(brandPayment.Id);
}
