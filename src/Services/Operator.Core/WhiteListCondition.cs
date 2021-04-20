using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    internal class WhiteListCondition : IBrandPaymentCondition
    {
        private readonly Guid[] _whiteList;

        public WhiteListCondition(IEnumerable<Guid> playerAllowBrandPaymentIds)
        {
            _whiteList = playerAllowBrandPaymentIds.ToArray();
        }

        public bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player)
            => _whiteList.Contains(brandPayment.Id);
    }
}
