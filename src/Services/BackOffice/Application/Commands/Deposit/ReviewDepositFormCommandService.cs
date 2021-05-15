using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.BackOffice.Domain;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.Operations;
using TGS.Services.Payment;
using TGS.Services.WalletService;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Commands.Deposit
{
    internal class ReviewDepositFormCommandService : ICommandService<ReviewDepositFormCommand>
    {
        private readonly IDepositRepository _depositRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IPlayerService _playerService;

        public ReviewDepositFormCommandService(
            IDepositRepository depositRepository,
            IWalletRepository walletRepository,
            IPlayerService playerService)
        {
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
            _walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
        }

        public async ValueTask ExecuteAsync(ReviewDepositFormCommand command, CancellationToken cancellationToken = default)
        {
            var form = await _depositRepository
                .GetDepositFormDetailAsync(command.FormId)
                .ConfigureAwait(false);

            await _depositRepository
                .SignOffDepositFormAsync(command.FormId, command.Status, command.remark)
                .ConfigureAwait(false);

            if (command.Status == ReviewState.Approve)
            {
                var player = await _playerService
                    .GetPlayerAsync(x => x.PlayerCode == form.PlayerCode)
                    .ConfigureAwait(false);

                await _walletRepository
                    .ExchangeAsync(
                        command.BrandId,
                        player.DefaultWalletId,
                        player.PlayerCode,
                        TransactionTypes.Deposit,
                        form.Amount)
                    .ConfigureAwait(false);
            }
        }
    }
}
