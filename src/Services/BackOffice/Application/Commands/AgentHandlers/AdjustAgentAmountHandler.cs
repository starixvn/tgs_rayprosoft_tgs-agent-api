using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.AgentAdjust;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.AgentHandlers
{
    public class AdjustAgentAmountHandler : Handler<CreateAgentAdjustmentRequest, MessageResponse>
    {
        private readonly IAgentService _agentService;
        private readonly IAgentTransactionService _agentTransactionService;

        public AdjustAgentAmountHandler(IAgentService agentService, IAgentTransactionService agentTransactionService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _agentService = agentService;
            _agentTransactionService = agentTransactionService;
        }

        public override async Task<MessageResponse> Handle(CreateAgentAdjustmentRequest request,
            CancellationToken cancellationToken)
        {
            var agent = await _agentService.GetAgentAsync(x => x.Id == request.AgentId);

            if (agent is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.NotFound);
            }

            var wallet = agent.Wallets.First(x => x.Id == agent.DefaultWalletId);

            if (wallet is null)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.WalletNotFound);
            }

            if (request.AdjustType == AdjustType.Subtract && wallet.Balance - request.Amount < 0)
            {
                return CreateInvalidResponse<MessageResponse>(Agent.BalanceMustGreaterThanZero);
            }

            await _agentTransactionService.CreateAgentTransactionAsync(agent, request);

            return CreateValidResponse<MessageResponse>();
        }
    }
}