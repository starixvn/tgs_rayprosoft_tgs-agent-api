using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Deposit
{
    internal class SubmitDepositFormThirdPartyOrderNoCommandService : ICommandService<SubmitDepositFormThirdPartyOrderNoCommand>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;

        public SubmitDepositFormThirdPartyOrderNoCommandService(IPaymentServiceClient paymentServiceClient)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public async ValueTask ExecuteAsync(SubmitDepositFormThirdPartyOrderNoCommand command, CancellationToken cancellationToken = default)
        {
            await _paymentServiceClient.SubmitDepositFormThirdPartyOrderNoAsync(command.FormId, command.ThirdPartyOrderNo);
        }
    }
}
