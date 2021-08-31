using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Tgs.Payment.Clients;
using TGS.Clients.Promotion.Client;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.PlayerBucket;
using TGS.Services.PlayerBucket.Models;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Commands.Deposit
{
    internal class ReviewDepositFormCommandService : ICommandService<ReviewDepositFormCommand>
    {
        private readonly ILogger<ReviewDepositFormCommandService> _logger;
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IPlayerBucketRepository _playerBucketRepository;
        private readonly IPlayerService _playerService;
        private readonly IPromotionClient _promotionClient;

        public ReviewDepositFormCommandService(
            IPaymentServiceClient paymentServiceClient,
            IPlayerService playerService,
            IPlayerBucketRepository playerBucketRepository,
            IPromotionClient promotionClient,
            ILogger<ReviewDepositFormCommandService> logger)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _playerBucketRepository = playerBucketRepository ?? throw new ArgumentNullException(nameof(playerBucketRepository));
            _promotionClient = promotionClient ?? throw new ArgumentNullException(nameof(promotionClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask ExecuteAsync(ReviewDepositFormCommand command, CancellationToken cancellationToken = default)
        {
            var form = await _paymentServiceClient
                .GetDepositFormAsync(command.BrandId, command.FormId)
                .ConfigureAwait(false);

            if (form.Status != PaymentFlowState.WaitApprove)
                return;

            await _paymentServiceClient
                .ReviewDepositFormAsync(command.FormId, command.Status, command.remark)
                .ConfigureAwait(false);

            if (command.Status == ReviewState.Approve)
            {
                var player = await _playerService
                    .GetPlayerAsync(x => x.PlayerCode == form.PlayerCode)
                    .ConfigureAwait(false);

                var actualAmount = await Policy.Handle<PostgresException>()
                    .RetryAsync(3, (ex, count) => _logger.LogWarning($"Retry occur {ex}"))
                    .ExecuteAsync(async () => await _playerBucketRepository
                        .CreatePlayerBucketWithDepositAsync(new CreatePlayerBucketWithDepositRequest(
                            command.BrandId,
                            form.PromotionId,
                            player.PlayerCode,
                            player.DefaultWalletId,
                            form.Amount))
                        .ConfigureAwait(false))
                    .ConfigureAwait(false);

                await _playerService
                    .TopUpAccumulatedDepositAsync(form.PlayerCode, form.Amount)
                    .ConfigureAwait(false);
            }
            else
            {
                if (form.PromotionId != null)
                    await _promotionClient
                        .RevertPromotionAsync(form.PromotionId.Value, form.Amount)
                        .ConfigureAwait(false);
            }
        }
    }
}
