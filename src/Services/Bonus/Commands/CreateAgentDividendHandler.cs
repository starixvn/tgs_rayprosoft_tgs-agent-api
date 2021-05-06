using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.Bonus.Models;

namespace TGS.Services.Bonus.Commands
{
    public class
        CreateAgentDividendHandler : Handler<SentAgentDividendsRequest,
            MessageResponse>
    {
        private readonly IAgentDividend _agentDividend;

        public CreateAgentDividendHandler(
            IAgentDividend agentDividend,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _agentDividend = agentDividend;
        }

        public override async Task<MessageResponse> Handle(SentAgentDividendsRequest request,
            CancellationToken cancellationToken)
        {
            if (request.AgentDividends.IsNullOrEmpty())
            {
                return CreateInvalidResponse<MessageResponse>("Agent dividends is null.");
            }

            await using var transaction =
                await _agentDividend.Repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await _agentDividend.SentAgentDividend(request.AgentDividends, request.TransactionType);
            await _agentDividend.UpdateAgentDividendSentOn(request.AgentDividends);

            await transaction.CommitAsync(cancellationToken);

            return CreateValidResponse<MessageResponse>();
        }
    }
}