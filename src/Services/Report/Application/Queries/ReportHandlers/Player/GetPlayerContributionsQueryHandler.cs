using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Player;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.ReportHandlers.Player
{
    public class
        GetPlayerContributionsQueryHandler : Handler<GetPlayerContributionsRequest,
            PagedResponse<PlayerContributionModel>>
    {
        private readonly IPlayerAggregateService _playerAggregateService;
        private readonly ITgsPlayerService _tgsPlayerService;

        public GetPlayerContributionsQueryHandler(IPlayerAggregateService playerAggregateService,
            ITgsPlayerService tgaPlayerService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _playerAggregateService = playerAggregateService;
            _tgsPlayerService = tgaPlayerService;
        }

        public override async Task<PagedResponse<PlayerContributionModel>> Handle(GetPlayerContributionsRequest request,
            CancellationToken cancellationToken)
        {
            var offset = new TimeSpan(8, 0, 0);
            var requestFrom = request.From.ToOffset(offset);
            var requestTo = request.To.ToOffset(offset);
            DateTimeOffset from = new(requestFrom.Year, requestFrom.Month, requestFrom.Day, 0, 0, 0,
                requestFrom.Offset);
            DateTimeOffset to = new(requestTo.Year, requestTo.Month, requestTo.Day, 0, 0, 0,
                requestTo.Offset);

            var players = await _tgsPlayerService.GetPlayerQueries(q => q.Where(x => x.BrandId == request.BrandId))
                .Include(x => x.Vip)
                .ToArrayAsync(cancellationToken);

            var tgsPlayer = request.Username.IsNotNullOrEmpty()
                ? null
                : players.FirstOrDefault(x =>
                    x.BrandId == request.BrandId && x.Username == request.Username);

            var tgsPlayers = request.Level == null
                ? null
                : players.Where(x => x.Vip.Level == request.Level)
                    .Select(x => x.PlayerCode).ToArray();

            var reportPlayers = await _playerAggregateService
                .GetPlayerAggregateQueries(q => q.Where(x => x.BrandId == request.BrandId)
                    .Where(x => x.ReportDate >= from && x.ReportDate <= to)
                    .Where(tgsPlayer != null, x => x.PlayerCode == tgsPlayer.PlayerCode)
                    .Where(tgsPlayers, x => tgsPlayers.Contains(x.PlayerCode)))
                .GroupBy(x => x.PlayerCode,
                    (playerCode, players) => new PlayerContributionModel
                    {
                        PlayerCode = playerCode,
                        Type = AccountType.Player,
                        DepositAmount = players.Sum(p => p.DepositAmount),
                        WithdrawalAmount = players.Sum(p => p.WithdrawalAmount),
                        AdjustmentAmount = players.Sum(p => p.AdjustmentAmount),
                        BonusAmount = players.Sum(p => p.BonusAmount),
                        BetAmount = players.Sum(p => p.BetAmount),
                        ValidBetAmount = players.Sum(p => p.Turnover),
                        WinLossAmount = players.Sum(p => p.Turnover),
                        NetProfit = players.Sum(p => p.NetProfit)
                    })
                .CreatePagedResponseAsync(request);

            foreach (var model in reportPlayers.Items)
            {
                var player = players.FirstOrDefault(x => x.PlayerCode == model.PlayerCode);
                model.Username = player?.Username;
                model.Level = player?.Vip?.Level;
            }

            return reportPlayers;
        }
    }
}