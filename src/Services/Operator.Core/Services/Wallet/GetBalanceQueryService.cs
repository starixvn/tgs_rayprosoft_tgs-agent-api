using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.CashService.Client;
using Tgs.ServiceUtility;
using TGS.Services.Operations;

namespace TGS.Services.Operator.Services.Wallet
{
    internal class GetBalanceQueryService : IGetService<GetBalanceQuery, BalanceInfo>
    {
        private readonly ICashServiceClient _cashServiceClient;
        private readonly IPlayerOperations _playerOperations;

        public GetBalanceQueryService(ICashServiceClient cashServiceClient, IPlayerOperations playerOperations)
        {
            _cashServiceClient = cashServiceClient ?? throw new ArgumentNullException(nameof(cashServiceClient));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask<BalanceInfo> GetAsync(GetBalanceQuery query, CancellationToken cancellationToken = default)
        {
            var playerData = await _playerOperations
                .GetCachedPlayerAsync(query.PlayerCode)
                .ConfigureAwait(false);
            var balance = await _cashServiceClient
                .GetBalanceAsync(playerData.DefaultWalletId)
                .ConfigureAwait(false);

            return new BalanceInfo(balance);
        }
    }
}
