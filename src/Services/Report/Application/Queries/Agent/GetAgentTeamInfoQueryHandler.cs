using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        public override async Task<PagedResponse<TeamInfoModel>> Handle(GetAgentTeamInfoRequest request,
            CancellationToken cancellationToken)
        {
            var reportPlayers = await _playerAggregateService
                .GetPlayerAggregateQueries(q => q.Where(x => x.AgentId == request.AgentId))
                .ToArrayAsync(cancellationToken);

            var playerCodes = reportPlayers.Select(r => r.PlayerCode).Distinct().ToArray();

            return await reportPlayers.GroupJoin(
                    _tgsPlayerService.GetPlayerQueries(q => q.Where(x => playerCodes.Contains(x.PlayerCode)))
                    , p => p.PlayerCode, tgsPlayer => tgsPlayer.PlayerCode,
                    (player, tgsPlayer) => new {player, tgsPlayer})
                .SelectMany(x => x.tgsPlayer.DefaultIfEmpty(),
                    (x, tgsPlayer) => new {x.player, tgsPlayer.Username, tgsPlayer.RegisteredOn})
                .GroupBy(x => new {x.Username, x.RegisteredOn}, (player, tgsPlayer) =>
                    new TeamInfoModel
                    {
                        Username = player.Username,
                        RegisteredOn = player.RegisteredOn,
                        Contribution = tgsPlayer.Sum(x => x.player.WinLossAmount)
                    }).CreatePagedResponseAsync(playerCodes.Length);
        }
    }
}