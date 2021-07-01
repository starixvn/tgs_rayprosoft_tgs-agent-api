using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.MessageQueue;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;

namespace TGS.Services.Promotion.MessageQueue
{
    public class DepositStatusChangedMessageHandler : IMessageHandler
    {
        private readonly IPromotionChecker _dailyFirstDepositChecker;
        private readonly IPromotionChecker _firstDepositChecker;
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IPlayerService _playerService;

        public DepositStatusChangedMessageHandler(
            IPaymentServiceClient paymentServiceClient,
            IPromotionCheckerFactory promotionCheckerFactory,
            IPlayerService playerService)
        {
            if (promotionCheckerFactory is null)
            {
                throw new ArgumentNullException(nameof(promotionCheckerFactory));
            }

            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _playerService = playerService ?? throw new ArgumentNullException(nameof(playerService));
            _firstDepositChecker = promotionCheckerFactory.GetPromotionChecker("FirstDeposit");
            _dailyFirstDepositChecker = promotionCheckerFactory.GetPromotionChecker("DailyFirstDeposit");
        }

        public async ValueTask HandleAsync(byte[] data, CancellationToken cancellationToken = default)
        {
            var formEvent = FormEvent.Parser.ParseFrom(data);

            if (formEvent == null)
                return;

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

                    if (depositForm.CreatedOn.Date == currentDate)
                        await _dailyFirstDepositChecker
                            .CommitTagAsync(depositForm.PlayerCode)
                            .ConfigureAwait(false);
                    break;

                case PaymentFlowState.Reject:
                    await _firstDepositChecker
                        .RollbackTagAsync(depositForm.PlayerCode)
                        .ConfigureAwait(false);

                    if (depositForm.CreatedOn.Date == currentDate)
                        await _dailyFirstDepositChecker
                            .RollbackTagAsync(depositForm.PlayerCode)
                            .ConfigureAwait(false);
                    break;
            }
        }
    }
}
