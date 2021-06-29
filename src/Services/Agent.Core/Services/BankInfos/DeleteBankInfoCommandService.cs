using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.BankInfos
{
    internal class DeleteBankInfoCommandService : ICommandService<DeleteBankInfoCommand>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;

        public DeleteBankInfoCommandService(IPaymentServiceClient paymentServiceClient)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
        }

        public ValueTask ExecuteAsync(DeleteBankInfoCommand command, CancellationToken cancellationToken = default)
            => _paymentServiceClient.DeleteAgentBankInfoAsync(command.AgentId, command.BankInfoId);
    }
}
