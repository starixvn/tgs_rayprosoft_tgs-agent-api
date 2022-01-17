using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.GameAction;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.GameAction
{
    internal class GetGameActionsQueryHandler : Handler<GetGameActionsRequest, PagedResponse<GameActionModel>>
    {
        private readonly IGameActionService _service;

        public GetGameActionsQueryHandler(IGameActionService service, IHttpContextAccessor accessor) :
            base(accessor)
            => _service = service;

        public override Task<PagedResponse<GameActionModel>> Handle(
            GetGameActionsRequest request,
            CancellationToken cancellationToken)
            => _service.GetGameActionQueries().Where(x => x.RoundId == request.RoundId)
                .Select(x => new GameActionModel
                {
                    RecordedOn = x.RecordedOn,
                    ExternalTransactionId = x.ExternalTransactionId,
                    GameActionType = x.Type,
                    Amount = x.Amount,
                    BeforeBalance = x.Balance - x.Amount,
                    AfterBalance = x.Balance
                })
                .OrderByDescending(x => x.RecordedOn)
                .CreatePagedResponseAsync(request);
    }
}