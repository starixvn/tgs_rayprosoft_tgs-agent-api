using System;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.BankDeposit
{
    internal class GetBrandPaymentQueryService : IGetService<GetBrandPaymentQuery, BrandPayment>
    {
        private readonly IDepositRepository _depositRepository;

        public GetBrandPaymentQueryService(IDepositRepository depositRepository)
        {
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
        }

        public ValueTask<BrandPayment> GetAsync(GetBrandPaymentQuery query)
        {
            return _depositRepository.GetBrandPaymentByIdAsync(query.Id);
        }
    }
}
