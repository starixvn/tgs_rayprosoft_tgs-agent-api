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

namespace TGS.Services.Report.Application.Queries.Vip
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

            var brandPlayers = await _tgsPlayerService.GetPlayerQueries(q => q.Where(x => x.BrandId == request.BrandId))
                .Include(x => x.Vip)
                .ToArrayAsync(cancellationToken);

            var tgsPlayer = request.Username.IsNotNullOrEmpty()
                ? brandPlayers.FirstOrDefault(x =>
                    x.BrandId == request.BrandId && x.Username == request.Username)
                : null;

            var tgsPlayers = request.Level == null
                ? null
                : brandPlayers.Where(x => x.Vip?.Level == request.Level)
                    .Select(x => x.PlayerCode).ToArray();

            var vips = await _service.GetPlayerAggregateQueries()
                .Where(x => x.BrandId == request.BrandId)
                .Where(x => x.ReportDate >= from && x.ReportDate < to)
                .Where(tgsPlayer != null, x => x.PlayerCode == tgsPlayer.PlayerCode)
                .GroupBy(x => new {x.PlayerCode, x.BrandId},
                    (x, players) => new
                    {
                        x.BrandId,
                        x.PlayerCode,
                        DepositAmount = players.Sum(m => m.DepositAmount),
                        WithdrawalAmount = players.Sum(m => m.WithdrawalAmount),
                        AdjustmentAmount = players.Sum(m => m.AdjustmentAmount),
                        BonusAmount = players.Sum(m => m.BonusAmount),
                        BetAmount = players.Sum(m => m.BetAmount),
                        Turnover = players.Sum(m => m.Turnover),
                        WinLossAmount = players.Sum(m => m.WinLossAmount),
                        NetProfit = players.Sum(m => m.NetProfit)
                    })
                .GroupJoin(_service.Repository.PlayerGameTypeAggregates
                        .Where(x => x.BrandId == request.BrandId)
                        .Where(x => x.ReportDate >= from && x.ReportDate < to)
                        .Where(tgsPlayer != null, x => x.PlayerCode == tgsPlayer.PlayerCode),
                    p => new {p.BrandId, p.PlayerCode},
                    pg => new {pg.BrandId, pg.PlayerCode},
                    (p, pg) => new {p, pg})
                .SelectMany(x => x.pg.DefaultIfEmpty(),
                    (x, pg) => new
                    {
                        x.p.BrandId,
                        x.p.PlayerCode,
                        x.p.DepositAmount,
                        x.p.WithdrawalAmount,
                        x.p.AdjustmentAmount,
                        x.p.BonusAmount,
                        x.p.BetAmount,
                        x.p.Turnover,
                        x.p.WinLossAmount,
                        x.p.NetProfit,
                        pg
                    })
                .Where(request.Level, x => tgsPlayers.Contains(x.PlayerCode))
                .GroupBy(x => new
                    {
                        x.BrandId,
                        x.PlayerCode,
                        x.DepositAmount,
                        x.WithdrawalAmount,
                        x.AdjustmentAmount,
                        x.BonusAmount,
                        x.BetAmount,
                        x.Turnover,
                        x.WinLossAmount,
                        x.NetProfit
                    },
                    (x, players) => new VipModel
                    {
                        BrandId = x.BrandId,
                        PlayerCode = x.PlayerCode,
                        DepositAmount = x.DepositAmount,
                        WithdrawalAmount = x.WithdrawalAmount,
                        AdjustmentAmount = x.AdjustmentAmount,
                        RebateAmount =
                            players.Sum(m =>
                                m.pg.VipRebateAmount),
                        BonusAmount = x.BonusAmount,
                        BetAmount = x.BetAmount,
                        Turnover = x.Turnover,
                        WinLossAmount = x.WinLossAmount,
                        NetProfit = x.NetProfit
                    }).ToArrayAsync(cancellationToken);

            return vips.Select(x =>
            {
                var player = brandPlayers.FirstOrDefault(p => p.PlayerCode == x.PlayerCode);

                return new VipModel
                {
                    Username = player?.Username,
                    Level = player?.Vip?.Level,
                    BrandId = x.BrandId,
                    PlayerCode = x.PlayerCode,
                    DepositAmount = x.DepositAmount,
                    WithdrawalAmount = x.WithdrawalAmount,
                    AdjustmentAmount = x.AdjustmentAmount,
                    RebateAmount = x.RebateAmount,
                    BonusAmount = x.BonusAmount,
                    BetAmount = x.BetAmount,
                    Turnover = x.Turnover,
                    WinLossAmount = x.WinLossAmount,
                    NetProfit = x.NetProfit
                };
            }).AsQueryable().CreatePagedResponse(request);
        }
    }
}