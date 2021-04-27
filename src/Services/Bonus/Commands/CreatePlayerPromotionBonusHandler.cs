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
        CreatePlayerPromotionBonusHandler : Handler<SentPlayerPromotionBonusRequest,
            MessageResponse>
    {
        private readonly IReport _reportPromotion;
        private readonly IPlayerBonus _sentBonus;

        public CreatePlayerPromotionBonusHandler(
            IPlayerBonus sentBonus,
            IReport reportPromotion,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _sentBonus = sentBonus;
            _reportPromotion = reportPromotion;
        }

        public override async Task<MessageResponse> Handle(SentPlayerPromotionBonusRequest request,
            CancellationToken cancellationToken)
        {
            if (request.PlayerBonus.IsNullOrEmpty())
            {
                return CreateInvalidResponse<MessageResponse>("Player bonus is null.");
            }

            await using var transaction =
                await _sentBonus.Repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await using var reportTransaction =
                await _reportPromotion.Repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await _sentBonus.SentPromotionBonus(request.PlayerBonus, request.TransactionType);
            await _reportPromotion.UpdatePromotionBonusSentOn(request.PlayerBonus);

            await transaction.CommitAsync(cancellationToken);
            await reportTransaction.CommitAsync(cancellationToken);

            return CreateValidResponse<MessageResponse>();
        }
    }
}