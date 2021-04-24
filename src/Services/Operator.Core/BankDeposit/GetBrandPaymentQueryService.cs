using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.BankDeposit
{
    internal class GetBrandPaymentQueryService : IGetService<GetBrandPaymentQuery, BrandPayment>
    {
        private readonly IWalletRepository _walletRepository;

        public GetBrandPaymentQueryService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public ValueTask<BrandPayment> GetAsync(GetBrandPaymentQuery query, CancellationToken cancellationToken = default)
        {
            return _walletRepository.GetBrandPaymentByIdAsync(query.Id);
        }
    }
}
