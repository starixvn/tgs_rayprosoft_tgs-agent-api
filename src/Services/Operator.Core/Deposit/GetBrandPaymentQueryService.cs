using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Deposit
{
    internal class GetBrandPaymentQueryService : IGetService<GetBrandPaymentQuery, BrandPaymentInfo>
    {
        private readonly IWalletRepository _walletRepository;

        public GetBrandPaymentQueryService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public ValueTask<BrandPaymentInfo> GetAsync(GetBrandPaymentQuery query, CancellationToken cancellationToken = default)
        {
            return _walletRepository.GetBrandPaymentByIdAsync(query.Id);
        }
    }
}
