using System.Linq;
using TGS.Services.Payment;
using TGS.Services.Operator.BrandPayment.PaymentConditions;
using TGS.Services.Operator;

namespace TGS.Services.Operator.BrandPayment.PaymentConditions
{
    internal class MatchOneCondition : IBrandPaymentCondition
    {
        private readonly IBrandPaymentCondition[] _brandPaymentConditions;

        public MatchOneCondition(params IBrandPaymentCondition[] brandPaymentConditions)
        {
            _brandPaymentConditions = brandPaymentConditions;
        }

        public bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player)
            => _brandPaymentConditions.Any(condition => condition.IsValid(brandPayment, player));
    }
}
