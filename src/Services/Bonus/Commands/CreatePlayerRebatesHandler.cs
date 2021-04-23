using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Bonus.Models;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;

namespace Bonus.Commands
{
    public class
        CreatePlayerBonusHandler : Handler<SentPlayerRebatesRequest,
            MessageResponse>
    {
        private readonly IReport _report;
        private readonly IPlayerBonus _sentBonus;

        public CreatePlayerBonusHandler(
            IPlayerBonus sentBonus,
            IReport report,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _sentBonus = sentBonus;
            _report = report;
        }

        public override async Task<MessageResponse> Handle(SentPlayerRebatesRequest request,
            CancellationToken cancellationToken)
        {
            await using var transaction =
                await _sentBonus.Repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await using var reportTransaction =
                await _report.Repository.BeginTransactionAsync(IsolationLevel.ReadCommitted);

            await _sentBonus.SentBonus(request.PlayerRebates, request.TransactionType);
            await _report.UpdatePlayerRebateSentOn(request.PlayerRebates);

            await transaction.CommitAsync(cancellationToken);
            await reportTransaction.CommitAsync(cancellationToken);

            return CreateValidResponse<MessageResponse>();
        }
    }
}