using System;
using System.Collections.Generic;
using System.Linq;
using TGS.Services.Payment;
using TGS.Services.Operator.BrandPayment.PaymentConditions;
using TGS.Services.Operator;

namespace TGS.Services.Operator.BrandPayment.PaymentConditions
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
