using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Vip;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.Vip
{
    internal class GetVipRebateReportQueryHandler : Handler<GetVipRebateReportRequest, PagedResponse<VipRebateModel>>
    {
        private readonly IPlayerGameTypeAggregateService _service;
        private readonly ITgsPlayerService _tgsPlayerService;

        public GetVipRebateReportQueryHandler(IPlayerGameTypeAggregateService service,
            ITgsPlayerService tgsPlayerService,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _tgsPlayerService = tgsPlayerService;
        }

        public override async Task<PagedResponse<VipRebateModel>> Handle(
            GetVipRebateReportRequest request,
            CancellationToken cancellationToken)
        {
            var offset = new TimeSpan(8, 0, 0);
            var requestFrom = request.From.ToOffset(offset);
            var requestTo = request.To.ToOffset(offset);
            DateTimeOffset from = new(requestFrom.Year, requestFrom.Month, requestFrom.Day, 0, 0, 0,
                requestFrom.Offset);
            DateTimeOffset to = new(requestTo.Year, requestTo.Month, requestTo.Day, 0, 0, 0,
                requestTo.Offset);

            var tgsPlayer =
                await _tgsPlayerService.GetPlayerAsync(x =>
                    x.BrandId == request.BrandId && x.Username == request.Username);

            return await _service.GetPlayerGameTypeAggregateQueries()
                .Where(x => x.BrandId == request.BrandId)
                .Where(x => x.ReportDate >= from && x.ReportDate < to)
                .Where(tgsPlayer != null, x => x.PlayerCode == tgsPlayer.PlayerCode)
                .GroupBy(
                    x => new
                    {
                        x.ReportDate,
                        LiveDealerRebatePercentage = x.VipLiveDealerRebatePercentage,
                        CardGameRebatePercentage = x.VipCardGameRebatePercentage,
                        LotteryRebatePercentage = x.VipLotteryRebatePercentage,
                        SlotRebatePercentage = x.VipSlotRebatePercentage,
                        SportsRebatePercentage = x.VipSportsRebatePercentage,
                        VirtualSportsRebatePercentage = x.VipVirtualSportsRebatePercentage
                    }, (x, players) =>
                        new VipRebateModel
                        {
                            ReportDate = x.ReportDate,
                            LiveDealerRebatePercentage = x.LiveDealerRebatePercentage,
                            LiveDealerAmount =
                                x.LiveDealerRebatePercentage * players.Where(p => p.GameType == GameType.LiveDealer)
                                    .Sum(m => m.Turnover) / 100,
                            LiveDealerTurnover =
                                players.Where(p => p.GameType == GameType.LiveDealer).Sum(m => m.Turnover),
                            SlotRebatePercentage = x.SlotRebatePercentage,
                            SlotAmount =
                                x.SlotRebatePercentage * players.Where(p =>
                                    p.GameType == GameType.Fishing || p.GameType == GameType.Arcade ||
                                    p.GameType == GameType.Slot || p.GameType == GameType.TableGame).Sum(m =>
                                    m.Turnover) / 100,
                            SlotTurnover =
                                players.Where(p =>
                                    p.GameType == GameType.Fishing || p.GameType == GameType.Arcade ||
                                    p.GameType == GameType.Slot || p.GameType == GameType.TableGame).Sum(m =>
                                    m.Turnover),
                            LotteryRebatePercentage = x.LotteryRebatePercentage,
                            LotteryAmount =
                                x.LotteryRebatePercentage *
                                players.Where(p => p.GameType == GameType.Lottery).Sum(m => m.Turnover) / 100,
                            LotteryTurnover = players.Where(p => p.GameType == GameType.Lottery).Sum(m => m.Turnover),
                            CardGameRebatePercentage = x.CardGameRebatePercentage,
                            CardGameAmount =
                                x.CardGameRebatePercentage * players.Where(p => p.GameType == GameType.CardGame)
                                    .Sum(m => m.Turnover) / 100,
                            CardGameTurnover = players.Where(p => p.GameType == GameType.CardGame).Sum(m => m.Turnover),
                            SportsRebatePercentage = x.SportsRebatePercentage,
                            SportsAmount =
                                x.SportsRebatePercentage * players.Where(p =>
                                        p.GameType == GameType.Sportsbook || p.GameType == GameType.ESports)
                                    .Sum(m => m.Turnover) /
                                100,
                            SportsTurnover =
                                players.Where(p => p.GameType == GameType.Sportsbook || p.GameType == GameType.ESports)
                                    .Sum(m => m.Turnover),
                            VirtualSportsRebatePercentage = x.VirtualSportsRebatePercentage,
                            VirtualSportsAmount =
                                x.VirtualSportsRebatePercentage *
                                players.Where(p => p.GameType == GameType.VirtualSports).Sum(m => m.Turnover) / 100,
                            VirtualSportsTurnover = players.Where(p => p.GameType == GameType.VirtualSports)
                                .Sum(m => m.Turnover)
                        })
                .CreatePagedResponseAsync(request);
        }
    }
}