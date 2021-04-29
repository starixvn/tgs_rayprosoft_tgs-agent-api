using System.Collections.Generic;
using MediatR;
using TGS.Core.Models.Bonus;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace Bonus.Models
{
    public record SentAgentDividendsRequest(IEnumerable<AgentDividend> AgentDividends,
        TransactionType TransactionType) : IRequest<MessageResponse>;
}