using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Commands.Withdraw
{
    internal class ReviewWithdrawFormCommandService : ICommandService<ReviewWithdrawFormCommand>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IWalletExchangeService _walletExchangeService;

        public ReviewWithdrawFormCommandService(
            IPaymentServiceClient paymentServiceClient,
            IWalletExchangeService walletExchangeService)
        {
            _paymentServiceClient =
                paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));

            _walletExchangeService =
                walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
        }

        public async ValueTask ExecuteAsync(
            ReviewWithdrawFormCommand command, CancellationToken cancellationToken = default)
        {
            var formData = await _paymentServiceClient
                .GetWithdrawFormAsync(command.BrandId, command.FormId)
                .ConfigureAwait(false);

            if (formData.Status != PaymentFlowState.WaitApprove)
                return;

            if (command.Status == ReviewState.Approve)
            {
                await _paymentServiceClient
                    .ReviewWithdrawFormAsync(
                        command.FormId,
                        command.BrandPaymentId.Value,
                        command.Status,
                        command.ReviewUserId,
                        command.ReviewUserName,
                        command.Remark)
                    .ConfigureAwait(false);
            }
            else
            {
                await _paymentServiceClient
                    .ReviewWithdrawFormAsync(
                        command.FormId,
                        command.BrandPaymentId.Value,
                        command.Status,
                        command.ReviewUserId,
                        command.ReviewUserName,
                        command.Remark)
                    .ConfigureAwait(false);

                await _walletExchangeService.CancelWithdrawAsync(
                    formData.BrandId,
                    formData.UserType,
                    formData.UserId,
                    formData.Amount,
                    command.ReviewUserId).ConfigureAwait(false);
            }
        }
    }
}
