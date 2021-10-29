using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Services.Operations;
using TGS.Services.WalletService;

namespace TGS.Services.Operator.Services.Wallet
{
    internal class GetBalanceQueryService : IGetService<GetBalanceQuery, BalanceInfo>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IWalletRepository _walletRepository;

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
