using System.Collections.Generic;
using MediatR;
using TGS.Core.Models.Bonus;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.Bonus.Models
{
    public record SentPlayerRebatesRequest(IEnumerable<PlayerRebates> PlayerRebates,
        TransactionType TransactionType) : IRequest<MessageResponse>;
}