using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Bonus.Models;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;

namespace Bonus.Commands
{
    public class
        CreateAgentDividendHandler : Handler<SentAgentDividendsRequest,
            MessageResponse>
    {
        private readonly IAgentDividend _agentDividend;
        private readonly IReport _report;

        public CreateAgentDividendHandler(
            IAgentDividend agentDividend,
            IReport report,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _agentDividend = agentDividend;
            _report = report;
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
            await using var reportTransaction =
                await _report.Repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await _agentDividend.SentAgentDividend(request.AgentDividends, request.TransactionType);
            await _report.UpdateAgentDividendSentOn(request.AgentDividends);

            await transaction.CommitAsync(cancellationToken);
            await reportTransaction.CommitAsync(cancellationToken);

            return CreateValidResponse<MessageResponse>();
        }
    }
}