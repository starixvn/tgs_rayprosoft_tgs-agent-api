using System.Threading;
using System.Threading.Tasks;
using Bonus.Models;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;

namespace Bonus.Commands
{
    public class
        CreatePlayerBonusHandler : Handler<CreatePlayerBonusRequest,
            MessageResponse>
    {
        private readonly ISentBonus _sentBonus;

        public CreatePlayerBonusHandler(
            ISentBonus sentBonus,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _sentBonus = sentBonus;
        }

        public override async Task<MessageResponse> Handle(CreatePlayerBonusRequest request,
            CancellationToken cancellationToken)
        {
            await _sentBonus.Sent(request.PlayerBonus, request.TransactionType,
                request.IsPromotion);
            return CreateValidResponse<MessageResponse>();
        }
    }
}