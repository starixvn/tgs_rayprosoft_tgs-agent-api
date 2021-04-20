using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Deposit
{
    internal class GetPaymentQueryService : IGetService<GetPaymentQuery, PaymentInfo>
    {
        private readonly IWalletRepository _walletRepository;

        public GetPaymentQueryService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public ValueTask<PaymentInfo> GetAsync(GetPaymentQuery query, CancellationToken cancellationToken = default)
        {
            return _walletRepository.GetPaymentAsync(query.PaymentId);
        }
    }
}
