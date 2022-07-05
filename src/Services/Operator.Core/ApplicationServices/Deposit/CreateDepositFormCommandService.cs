using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using Tgs.Promotion.Clients;
using Tgs.ServiceUtility;

namespace TGS.Services.Operator.ApplicationServices.Deposit;

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
                throw new CannotJoinPromotionException();
        }

        var createFormId = Guid.NewGuid();

        try
        {
            await _promotionClient
                .TagPlayerDepositAsync(command.PlayerCode, command.PromotionId)
                .ConfigureAwait(false);

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
        }
        catch
        {
            await _promotionClient
                .UntagPlayerDepositAsync(command.PlayerCode, command.PromotionId)
                .ConfigureAwait(false);
            throw;
        }

        await _paymentServiceClient
            .SubmitDepositFormStatusAsync(createFormId, PaymentFlowState.WaitPayment, null)
            .ConfigureAwait(false);

        command.ReturnFormIdSource.SetResult(createFormId);
    }
}
