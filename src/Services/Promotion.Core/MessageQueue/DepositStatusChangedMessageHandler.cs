using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Tgs.MessageQueue;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;

namespace TGS.Services.Promotion.MessageQueue
{
    public class DepositStatusChangedMessageHandler : IMessageHandler
    {
        private readonly IPromotionChecker _dailyFirstDepositChecker;
        private readonly IPromotionChecker _firstDepositChecker;
        private readonly ILogger<DepositStatusChangedMessageHandler> _logger;
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IPlayerService _playerService;
        private readonly IPromotionParticipantRepository _promotionParticipantService;

        public DepositStatusChangedMessageHandler(
            IPaymentServiceClient paymentServiceClient,
            IPromotionCheckerFactory promotionCheckerFactory,
            IPlayerService playerService,
            IPromotionParticipantRepository promotionParticipantService,
            ILogger<DepositStatusChangedMessageHandler> logger)
        {
            if (promotionCheckerFactory is null)
            {
                throw new ArgumentNullException(nameof(promotionCheckerFactory));
            }

            _paymentServiceClient =
                paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _promotionParticipantService = promotionParticipantService ??
                throw new ArgumentNullException(nameof(promotionParticipantService));
            _firstDepositChecker = promotionCheckerFactory.GetPromotionChecker("FirstDeposit");
            _dailyFirstDepositChecker = promotionCheckerFactory.GetPromotionChecker("DailyFirstDeposit");
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deposit status change event.");

            var formEvent = FormEvent.Parser.ParseFrom(data);

            if (formEvent == null)
                return;

            _logger.LogInformation($"formEvent: ({JsonFormatter.Default.Format(formEvent)})");

            var depositForm = await _paymentServiceClient
                .GetDepositFormAsync(Guid.Parse(formEvent.BrandId), Guid.Parse(formEvent.FormId))
                .ConfigureAwait(false);

            if (depositForm == null)
                return;

            var currentDate = DateTimeOffset.Now.Date;

            switch (depositForm.Status)
            {
                case PaymentFlowState.Approved:
                    await _playerService
                        .SetFirstDepositTimeAsync(depositForm.PlayerCode, DateTimeOffset.Now)
                        .ConfigureAwait(false);

                    await _firstDepositChecker
                        .CommitTagAsync(depositForm.PlayerCode)
                        .ConfigureAwait(false);

                    if (depositForm.CreatedOn.ToLocalTime().Date == currentDate)
                        await _dailyFirstDepositChecker
                            .CommitTagAsync(depositForm.PlayerCode)
                            .ConfigureAwait(false);

                    if (depositForm.PromotionId.HasValue)
                    {
                        await _promotionParticipantService
                            .CreatePromotionParticipantAsync(depositForm.PromotionId.Value, depositForm.PlayerCode)
                            .ConfigureAwait(false);
                    }

                    break;

                case PaymentFlowState.Reject:
                    await _firstDepositChecker
                        .RollbackTagAsync(depositForm.PlayerCode)
                        .ConfigureAwait(false);

                    if (depositForm.CreatedOn.ToLocalTime().Date == currentDate)
                        await _dailyFirstDepositChecker
                            .RollbackTagAsync(depositForm.PlayerCode)
                            .ConfigureAwait(false);
                    break;
            }
        }
    }
}