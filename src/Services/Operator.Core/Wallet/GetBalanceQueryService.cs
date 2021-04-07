using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Wallet
{
    internal class GetBalanceQueryService : IGetService<GetBalanceQuery, BalanceInfo>
    {
        private readonly IWalletRepository _walletRepository;

        public GetBalanceQueryService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public ValueTask<BalanceInfo> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
        {
            return _walletRepository.GetBalanceInfoAsync(query.PlayerCode);
        }
    }
}
