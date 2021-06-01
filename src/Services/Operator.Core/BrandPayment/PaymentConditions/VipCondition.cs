using System;
using System.Collections.Generic;
using System.Linq;
using Tgs.Payment;

namespace TGS.Services.Operator.BrandPayment.PaymentConditions
{
    internal class VipCondition : IBrandPaymentCondition
    {
        private readonly IEnumerable<Guid> _allowVips;

        public VipCondition(IEnumerable<Guid> allowVips)
        {
            _allowVips = allowVips.ToArray();
        }

        public bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player)
        {
            if (player.VipLevel == null)
                return false;

            return _allowVips.Contains(player.VipLevel.Value);
        }
    }
}
