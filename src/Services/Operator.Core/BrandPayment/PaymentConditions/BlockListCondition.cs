using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Services.Payment;
using TGS.Services.Operator;
using TGS.Services.Operator.BrandPayment.PaymentConditions;

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
