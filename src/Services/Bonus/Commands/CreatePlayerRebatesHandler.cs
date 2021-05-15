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
        CreatePlayerBonusHandler : Handler<SentPlayerRebatesRequest,
            MessageResponse>
    {
        private readonly IPlayerBonus _sentBonus;

        public CreatePlayerBonusHandler(
            IPlayerBonus sentBonus,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _sentBonus = sentBonus;
        }

        public override async Task<MessageResponse> Handle(SentPlayerRebatesRequest request,
            CancellationToken cancellationToken)
        {
            if (request.PlayerRebates.IsNullOrEmpty())
            {
                return CreateInvalidResponse<MessageResponse>("Player rebates is null.");
            }

            await using var transaction =
                await _sentBonus.Repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await _sentBonus.SentBonus(request.PlayerRebates, request.TransactionType);
            await _sentBonus.UpdatePlayerRebateSentOn(request.PlayerRebates);

            await transaction.CommitAsync(cancellationToken);

            return CreateValidResponse<MessageResponse>();
        }
    }
}