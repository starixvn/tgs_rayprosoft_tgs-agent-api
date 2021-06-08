using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Payment;
using TGS.ServiceUtility;

namespace TGS.Services.Agent.Services.BankInfos
{
    internal class CreateBankInfoCommandService : ICommandService<CreateBankInfoCommand>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IPaymentRepository _paymentRepository;

        public CreateBankInfoCommandService(
            IPaymentRepository paymentRepository,
            IAgentRepository agentRepository)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _agentRepository = agentRepository ?? throw new ArgumentNullException(nameof(agentRepository));
        }

        public async ValueTask ExecuteAsync(CreateBankInfoCommand command, CancellationToken cancellationToken = default)
        {
            var agent = await _agentRepository.GetAgentByIdAsync(command.AgentId).ConfigureAwait(false);

            var bankInfo = new AgentBankInfo(
                Guid.NewGuid(),
                command.AgentId,
                command.BrandId,
                command.PaymentChannelId,
                command.BankType,
                command.Detail);

            await _paymentRepository
                .CreateAgentBankInfoAsync(bankInfo, agent.UserName)
                .ConfigureAwait(false);
        }
    }
}
