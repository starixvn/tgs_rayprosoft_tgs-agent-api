using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.Services.WalletService;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Wallet
{
    internal class GetBalanceQueryService : IGetService<GetBalanceQuery, BalanceInfo>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IPlayerOperations _playerOperations;

        public GetBalanceQueryService(IWalletRepository walletRepository, IPlayerOperations playerOperations)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask<BalanceInfo> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var playerData = await _playerOperations
                .GetCachedPlayerAsync(query.PlayerCode)
                .ConfigureAwait(false);
            var balance = await _walletRepository
                .GetBalanceAsync(playerData.DefaultWalletId, query.PlayerCode)
                .ConfigureAwait(false);

            return new BalanceInfo(balance);
        }
    }
}
