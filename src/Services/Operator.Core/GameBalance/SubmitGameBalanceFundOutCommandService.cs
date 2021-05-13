using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.Operations;
using TGS.Services.WalletService;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.GameBalance
{
    internal class SubmitGameBalanceFundOutCommandService : ICommandService<SubmitGameBalanceFundOutCommand>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IWalletRepository _walletRepository;

        public SubmitGameBalanceFundOutCommandService(IPlayerOperations playerOperations, IWalletRepository walletRepository)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }

        public async ValueTask ExecuteAsync(SubmitGameBalanceFundOutCommand command, CancellationToken cancellationToken = default)
        {
            var playerData = await _playerOperations
                .GetCachedPlayerAsync(command.PlayerCode)
                .ConfigureAwait(false);
            var balance = await _walletRepository
                .GetBalanceAsync(playerData.DefaultWalletId, command.PlayerCode)
                .ConfigureAwait(false);
        }
    }
}