using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Player;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.Player
{
    public class
        GetPlayersAccountChangeQueryHandler : Handler<GetPlayersAccountChangesRequest,
            PagedResponse<PlayersAccountChangeModel>>
    {
        private readonly ITransactionService _transactionService;

        public GetPlayersAccountChangeQueryHandler(
            ITransactionService transactionService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _transactionService = transactionService;
        }

        public override Task<PagedResponse<PlayersAccountChangeModel>> Handle(
            GetPlayersAccountChangesRequest request,
            CancellationToken cancellationToken)
            => _transactionService.GetTransactionQueries().Where(x => x.BrandId == request.BrandId)
                .Where(x => x.RecordedOn >= request.From && x.RecordedOn < request.To)
                .Where(request.TransactionType is not null, x => x.Type == request.TransactionType)
                .Where(x => x.Type == TransactionType.Adjust || x.Type == TransactionType.Rebate ||
                    x.Type == TransactionType.Promotion)
                .Join(_transactionService.TgsRepository.Players,
                    transaction => transaction.PlayerCode,
                    player => player.PlayerCode,
                    (transaction, player) => new
                    {
                        Transaction = transaction,
                        Player = player
                    })
                .Where(request.Username.IsNotNullOrEmpty(), x => x.Player.Username == request.Username)
                .GroupJoin(_transactionService.TgsRepository.Users,
                    group => group.Transaction.CreatedByUserId,
                    user => user.Id,
                    (group, user) => new
                    {
                        Group = group,
                        User = user
                    })
                .SelectMany(x => x.User.DefaultIfEmpty(),
                    (x, user) => new
                    {
                        x.Group.Transaction,
                        x.Group.Player,
                        User = user
                    })
                .Select(x => new PlayersAccountChangeModel
                {
                    Username = x.Player.Username,
                    Amount = x.Transaction.Amount,
                    BeforeBalance = x.Transaction.Balance,
                    AfterBalance = x.Transaction.Balance + x.Transaction.Amount,
                    TransactionType = x.Transaction.Type,
                    RecordedOn = x.Transaction.RecordedOn,
                    Operator = x.User.Username ?? "System",
                    Description = x.Transaction.Description
                }).CreatePagedResponseAsync(request);
    }
}