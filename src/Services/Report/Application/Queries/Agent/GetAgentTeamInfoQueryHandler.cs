using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Agent;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.Agent
{
    public class GetAgentTeamInfoQueryHandler : Handler<GetAgentTeamInfoRequest, PagedResponse<TeamInfoModel>>
    {
        private readonly IPlayerAggregateService _playerAggregateService;
        private readonly ITgsPlayerService _tgsPlayerService;

        public GetAgentTeamInfoQueryHandler(IPlayerAggregateService playerAggregateService,
            ITgsPlayerService tgaPlayerService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _playerAggregateService = playerAggregateService;
            _tgsPlayerService = tgaPlayerService;
        }

        public override Task<PagedResponse<TeamInfoModel>> Handle(GetAgentTeamInfoRequest request,
            CancellationToken cancellationToken) =>
            _tgsPlayerService.GetPlayerQueries().Where(x => x.AgentId == request.AgentId).GroupJoin(
                    _playerAggregateService
                        .GetPlayerAggregateQueries(q => q.Where(x => x.AgentId == request.AgentId)), p => p.PlayerCode,
                    a => a.PlayerCode, (player, playerAggregate) => new {player, playerAggregate})
                .SelectMany(x => x.playerAggregate.DefaultIfEmpty(),
                    (x, playerAggregate) => new {x.player.Username, x.player.RegisteredOn, playerAggregate})
                .GroupBy(x => new {x.Username, x.RegisteredOn}, (player, playerAggregate) =>
                    new TeamInfoModel
                    {
                        Username = player.Username,
                        RegisteredOn = player.RegisteredOn,
                        Contribution = playerAggregate.Sum(x => x.playerAggregate.WinLossAmount)
                    }).CreatePagedResponseAsync(request);
    }
}