using System;
using System.Collections.Generic;
using System.Linq;

namespace TGS.Services.Operator
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
