using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Commands.Withdraw
{
    internal class PaidWithdrawFormCommandService : ICommandService<PaidWithdrawFormCommand>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;

        public PaidWithdrawFormCommandService(IPaymentServiceClient paymentServiceClient)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public async ValueTask ExecuteAsync(PaidWithdrawFormCommand command, CancellationToken cancellationToken = default)
        {
            var formData = await _paymentServiceClient
                .GetWithdrawFormAsync(command.BrandId, command.FormId)
                .ConfigureAwait(false);

            if (formData.Status != PaymentFlowState.Approved)
                return;

            await _paymentServiceClient
                .SubmitWithdrawFormStatusAsync(formData.Id, PaymentFlowState.Paid)
                .ConfigureAwait(false);
        }
    }
}
