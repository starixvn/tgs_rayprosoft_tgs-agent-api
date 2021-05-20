using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment;
using TGS.Clients.Promotion.Client;
using TGS.Services.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Deposit
{
    internal class CreateDepositFormCommandService : ICommandService<CreateDepositFormCommand>
    {
        private readonly IPaymentClient _paymentClient;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPromotionClient _promotionClient;

        public CreateDepositFormCommandService(
            IPaymentRepository paymentRepository,
            IPaymentClient paymentClient,
            IPromotionClient promotionClient)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
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
                    command.PromotionId.Value).ConfigureAwait(false);

                if (!canJoin)
                    throw new CannotJoinPromotionException();
            }
            var createFormId = Guid.NewGuid();

            var settings = command.CreatePaymentSettings(createFormId);
            var applyInfo = await _paymentClient
                .DepositAsync(command.Subject, command.Amount, settings)
                .ConfigureAwait(false);

            var form = new DepositFormInfo(
                command.PlayerCode,
                command.BrandId,
                command.BrandPaymentId,
                command.Amount,
                applyInfo);

            await _paymentRepository
               .CreateDepositFormAsync(createFormId, form)
               .ConfigureAwait(false);

            await _promotionClient
                .TagPlayerDepositAsync(command.PlayerCode)
                .ConfigureAwait(false);

            command.ReturnFormIdSource.SetResult(createFormId);
        }
    }
}
