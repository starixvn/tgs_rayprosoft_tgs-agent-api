using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.BackOffice.Domain;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.Payment;
using TGS.Services.WalletService;
using TGS.ServiceUtility;
using TGS.Services.PlayerBucket.Models;
using TGS.Services.PlayerBucket;

namespace TGS.Services.BackOffice.Application.Commands.Deposit
{
    internal class ReviewDepositFormCommandService : ICommandService<ReviewDepositFormCommand>
    {
        private readonly IDepositRepository _depositRepository;
        private readonly IPlayerBucketRepository _playerBucketRepository;
        private readonly IPlayerService _playerService;

        public ReviewDepositFormCommandService(
            IDepositRepository depositRepository,
            IPlayerService playerService,
            IPlayerBucketRepository playerBucketRepository)
        {
            _depositRepository = depositRepository ?? throw new ArgumentNullException(nameof(depositRepository));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _playerBucketRepository = playerBucketRepository ?? throw new ArgumentNullException(nameof(playerBucketRepository));
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

                await _playerBucketRepository
                    .CreatePlayerBucketWithDepositAsync(new CreatePlayerBucketWithDepositRequest(
                        command.BrandId,
                        null,
                        player.PlayerCode,
                        player.DefaultWalletId,
                        form.Amount))
                    .ConfigureAwait(false);
            }
        }
    }
}
