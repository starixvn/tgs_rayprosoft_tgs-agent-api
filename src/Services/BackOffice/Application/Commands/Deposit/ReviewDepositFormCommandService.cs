using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.BackOffice.Domain;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.Payment;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Commands.Deposit
{
    internal class ReviewDepositFormCommandService : ICommandService<ReviewDepositFormCommand>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPlayerBucketRepository _playerBucketRepository;
        private readonly IPlayerService _playerService;

        public ReviewDepositFormCommandService(
            IPaymentRepository paymentRepository,
            IPlayerService playerService,
            IPlayerBucketRepository playerBucketRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _playerBucketRepository = playerBucketRepository ?? throw new ArgumentNullException(nameof(playerBucketRepository));
        }

        public async ValueTask ExecuteAsync(ReviewDepositFormCommand command, CancellationToken cancellationToken = default)
        {
            var form = await _paymentRepository
                .GetDepositFormByIdAsync(command.FormId)
                .ConfigureAwait(false);

            await _paymentRepository
                .SignOffDepositFormAsync(command.FormId, command.Status, command.remark)
                .ConfigureAwait(false);

            if (command.Status == ReviewState.Approve)
            {
                var player = await _playerService
                    .GetPlayerAsync(x => x.PlayerCode == form.PlayerCode)
                    .ConfigureAwait(false);

                var actualAmount = await _playerBucketRepository
                    .CreatePlayerBucketWithDepositAsync(new CreatePlayerBucketWithDepositRequest(
                        command.BrandId,
                        null,
                        player.PlayerCode,
                        player.DefaultWalletId,
                        form.Amount))
                    .ConfigureAwait(false);

                await _playerService
                    .TopUpAccumulatedDepositAsync(form.PlayerCode, form.Amount)
                    .ConfigureAwait(false);
            }
        }
    }
}
