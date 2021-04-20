using System.Linq;

namespace TGS.Services.Operator
{
    internal class SomeOneMatchCondition : IBrandPaymentCondition
    {
        private readonly IBrandPaymentCondition[] _brandPaymentConditions;

        public SomeOneMatchCondition(params IBrandPaymentCondition[] brandPaymentConditions)
        {
            _brandPaymentConditions = brandPaymentConditions;
        }

        public bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player)
            => _brandPaymentConditions.Any(condition => condition.IsValid(brandPayment, player));
    }
}
