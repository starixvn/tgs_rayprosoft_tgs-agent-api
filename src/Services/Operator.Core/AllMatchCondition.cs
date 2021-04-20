using System.Linq;

namespace TGS.Services.Operator
{
    internal class AllMatchCondition : IBrandPaymentCondition
    {
        private readonly IBrandPaymentCondition[] _brandPaymentConditions;

        public AllMatchCondition(params IBrandPaymentCondition[] brandPaymentConditions)
        {
            _brandPaymentConditions = brandPaymentConditions;
        }

        public bool IsValid(BrandPaymentInfo brandPayment, PlayerInfo player)
            => _brandPaymentConditions.All(condition => condition.IsValid(brandPayment, player));
    }
}
