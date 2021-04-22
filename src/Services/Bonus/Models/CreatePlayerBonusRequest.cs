using MediatR;
using TGS.Core.Models.Bonus;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace Bonus.Models
{
    public record CreatePlayerBonusRequest(PlayerBonus[] PlayerBonus, TransactionType TransactionType,
        bool IsPromotion = false) : IRequest<MessageResponse>;
}