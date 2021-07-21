using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment.Clients;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.BankInfos
{
    internal class CreateBankInfoCommandService : ICommandService<CreateBankInfoCommand>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IPaymentServiceClient _paymentServiceClient;

        public CreateBankInfoCommandService(
            IPaymentServiceClient paymentServiceClient,
            IAgentRepository agentRepository)
        {
            _paymentServiceClient = paymentServiceClient ?? throw new ArgumentNullException(nameof(paymentServiceClient));
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        }

        public async ValueTask ExecuteAsync(CreateBankInfoCommand command, CancellationToken cancellationToken = default)
        {
            var agent = await _agentRepository.GetAgentByIdAsync(command.AgentId).ConfigureAwait(false);

            var createInfo = new BankCreateInfo
            {
                PaymentChannelId = command.PaymentChannelId,
                BankType = command.BankType,
                Detail = JsonDocument.Parse(command.Detail)
            };

            await _paymentServiceClient
                .CreateAgentBankInfoAsync(
                    command.BrandId,
                    command.AgentId,
                    agent.UserName,
                    createInfo)
                .ConfigureAwait(false);
        }
    }
}
