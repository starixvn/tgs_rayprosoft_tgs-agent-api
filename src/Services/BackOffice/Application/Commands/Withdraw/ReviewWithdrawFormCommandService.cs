using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.Services.BackOffice.Domain;
using TGS.Services.WalletService;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Commands.Withdraw
{
    internal class ReviewWithdrawFormCommandService : ICommandService<ReviewWithdrawFormCommand>
    {
        private readonly IPaymentClient _paymentClient;
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IWalletExchangeService _walletExchangeService;

        public ReviewWithdrawFormCommandService(
            IPaymentServiceClient paymentServiceClient,
            IPaymentClient paymentClient,
            IWalletExchangeService walletExchangeService)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _paymentClient = paymentClient ?? throw new ArgumentNullException(nameof(paymentClient));
            _walletExchangeService = walletExchangeService ?? throw new ArgumentNullException(nameof(walletExchangeService));
        }

        public async ValueTask ExecuteAsync(ReviewWithdrawFormCommand command, CancellationToken cancellationToken = default)
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
                        command.Remark)
                    .ConfigureAwait(false);

                await _walletExchangeService.ExchangeAsync(
                    formData.BrandId,
                    formData.UserType,
                    formData.UserId,
                    TransactionTypes.CancelWithdrawal,
                    formData.Amount).ConfigureAwait(false);
            }
        }
    }
}
