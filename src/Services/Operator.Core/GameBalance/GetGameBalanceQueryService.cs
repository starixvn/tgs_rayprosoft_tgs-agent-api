using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.Services.Operator.GameBalance;
using TGS.Services.WalletService;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Wallet
{
    internal class GetGameBalanceQueryService : IGetService<GetGameBalanceQuery, GameBalanceInfo>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IPlayerOperations _playerOperations;

        public GetGameBalanceQueryService(IWalletRepository walletRepository, IPlayerOperations playerOperations)
        {
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        //public async ValueTask<BalanceInfo> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
        //{
        //    var playerData = await _playerOperations
        //        .GetCachedPlayerAsync(query.PlayerCode)
        //        .ConfigureAwait(false);
        //    var balance = await _walletRepository
        //        .GetBalanceAsync(playerData.DefaultWalletId, query.PlayerCode)
        //        .ConfigureAwait(false);

        //    return new BalanceInfo(balance);
        //}

        public ValueTask<GameBalanceInfo> GetAsync(GetGameBalanceQuery query, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
            //var playerData = await _playerOperations
            //    .GetCachedPlayerAsync(query.PlayerCode)
            //    .ConfigureAwait(false);
            //var balance = await _walletRepository
            //    .GetBalanceAsync(playerData.DefaultWalletId, query.PlayerCode)
            //    .ConfigureAwait(false);

            //return new BalanceInfo(balance);
        }
    }
}
