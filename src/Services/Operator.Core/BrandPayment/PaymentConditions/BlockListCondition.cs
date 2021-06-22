using System;
using System.Collections.Generic;
using System.Linq;
using Tgs.Payment;
using Tgs.Payment.Clients;

namespace TGS.Services.Operator.BrandPayment.PaymentConditions
{
    internal class BlockListCondition : IBrandPaymentCondition
    {
        private Guid[] _blockList;

        public BlockListCondition(IEnumerable<Guid> playerBlockedBrandPaymentIds)
        {
            _blockList = playerBlockedBrandPaymentIds.ToArray();
        }

        public bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player)
            => !_blockList.Contains(brandPayment.Id);
    }
}
