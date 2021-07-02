using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.Clients.Promotion.Client;
using TGS.Services.Operations.Commands;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Deposit
{
    internal class CreateDepositFormCommandService : ICommandService<CreateDepositFormCommand>
    {
        private readonly IPaymentClient _paymentClient;
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IPromotionClient _promotionClient;

        public CreateDepositFormCommandService(
            IPaymentServiceClient paymentServiceClient,
            IPaymentClient paymentClient,
            IPromotionClient promotionClient)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
            _promotionClient = promotionClient ?? throw new ArgumentNullException(nameof(promotionClient));
        }

        public async ValueTask ExecuteAsync(
            CreateDepositFormCommand command,
            CancellationToken cancellationToken = default)
        {
            if (command.PromotionId != null)
            {
                var canJoin = await _promotionClient.CheckCanJoinDepositPromotionAsync(
                    command.BrandId,
                    command.PlayerCode,
                    command.PromotionId.Value,
                    command.Amount).ConfigureAwait(false);

                if (!canJoin)
                {
                    throw new CannotJoinPromotionException();
                }
            }

            await _promotionClient
                .TagPlayerDepositAsync(command.PlayerCode)
                .ConfigureAwait(false);

            var createFormId = Guid.NewGuid();

            var settings = command.CreatePaymentSettings(createFormId);
            var applyInfo = await _paymentClient
                .DepositAsync(command.Subject, command.Amount, settings)
                .ConfigureAwait(false);

            var form = new DepositFormCreateInfo
            {
                Amount = command.Amount,
                ApplyInfo = applyInfo,
                BrandId = command.BrandId,
                BrandPaymentId = command.BrandPaymentId,
                PlayerCode = command.PlayerCode,
                PromotionId = command.PromotionId
            };

            await _paymentServiceClient
                .CreateDepositFormAsync(createFormId, form)
                .ConfigureAwait(false);

            await _paymentServiceClient
                .SubmitDepositFormStatusAsync(createFormId, PaymentFlowState.WaitPayment, null)
                .ConfigureAwait(false);

            command.ReturnFormIdSource.SetResult(createFormId);
        }
    }
}
