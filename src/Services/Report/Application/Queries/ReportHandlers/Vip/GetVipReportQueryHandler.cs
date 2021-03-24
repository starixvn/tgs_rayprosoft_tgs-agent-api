using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Vip;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.ReportHandlers.Vip
{
    internal class GetVipReportQueryHandler : Handler<GetVipReportRequest, PagedResponse<VipModel>>
    {
        private readonly IPlayerAggregateService _service;
        private readonly ITgsPlayerService _tgsPlayerService;

        public GetVipReportQueryHandler(IPlayerAggregateService service, ITgsPlayerService tgsPlayerService,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _tgsPlayerService = tgsPlayerService;
        }

        public override async Task<PagedResponse<VipModel>> Handle(
            GetVipReportRequest request,
            CancellationToken cancellationToken)
        {
            var offset = new TimeSpan(8, 0, 0);
            var requestFrom = request.From.ToOffset(offset);
            var requestTo = request.To.ToOffset(offset);
            DateTimeOffset from = new(requestFrom.Year, requestFrom.Month, requestFrom.Day, 0, 0, 0,
                requestFrom.Offset);
            DateTimeOffset to = new(requestTo.Year, requestTo.Month, requestTo.Day, 0, 0, 0,
                requestTo.Offset);

            var tgsPlayer = request.Username.IsNotNullOrEmpty()
                ? null
                : await _tgsPlayerService.GetPlayerAsync(x => x.Username == request.Username);

            var tgsPlayers = request.Level != null
                ? await _tgsPlayerService.GetPlayerQueries(q => q.Where(x => x.Vip.Level == request.Level))
                    .Select(x => x.PlayerCode).ToArrayAsync(cancellationToken)
                : null;

            var playerAggregate = await _service.GetPlayerAggregateQueries().Join(
                    _service.Repository.PlayerGameTypeAggregates, p => new {p.BrandId, p.PlayerCode, p.ReportDate},
                    pg => new {pg.BrandId, pg.PlayerCode, pg.ReportDate},
                    (p, pg) => new {p, pg})
                .Where(x => x.p.BrandId == request.BrandId)
                .Where(x => x.p.ReportDate >= from && x.p.ReportDate <= to)
                .Where(tgsPlayer != null, x => x.p.PlayerCode == tgsPlayer.PlayerCode)
                .Where(tgsPlayers, x => tgsPlayers.Contains(x.p.PlayerCode)).ToArrayAsync(cancellationToken);

            var brandPlayers = await _tgsPlayerService.GetPlayerQueries(q => q.Where(x => x.BrandId == request.BrandId))
                .Include(x => x.Vip)
                .ToArrayAsync(cancellationToken);

            var vipModels = playerAggregate
                .Join(brandPlayers, x => x.p.PlayerCode, p => p.PlayerCode,
                    (pa, p) => new {pa.p, pa.pg, brandPlayer = p})
                .GroupBy(x => new {player = x.brandPlayer}, (x, players) =>
                    new VipModel
                    {
                        Username = x.player.Username,
                        Level = x.player.Vip.Level,
                        DepositAmount = players.Sum(m => m.p.DepositAmount),
                        WithdrawalAmount = players.Sum(m => m.p.WithdrawalAmount),
                        AdjustmentAmount = players.Sum(m => m.p.AdjustmentAmount),
                        RebateAmount =
                            players.Sum(m =>
                                m.pg.LiveDealerAmount + m.pg.CardGameAmount + m.pg.SlotAmount + m.pg.SportsAmount +
                                m.pg.VirtualSportsAmount),
                        BonusAmount = players.Sum(m => m.p.BonusAmount),
                        BetAmount = players.Sum(m => m.p.BetAmount),
                        ValidBetAmount = players.Sum(m => m.p.ValidBetAmount),
                        WinLossAmount = players.Sum(m => m.p.WinLossAmount),
                        NetProfit = players.Sum(m => m.p.NetProfit)
                    });

            return await vipModels.CreatePagedResponseAsync(vipModels.Count());
        }
    }
}