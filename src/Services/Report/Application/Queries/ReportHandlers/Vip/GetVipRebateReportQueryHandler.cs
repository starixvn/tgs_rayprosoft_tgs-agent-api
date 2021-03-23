using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Vip;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.ReportHandlers.Vip
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

            var tgsPlayer = await _tgsPlayerService.GetPlayerAsync(x => x.Username == request.Username);

            var playerAggregate = await _service.GetPlayerGameTypeAggregateQueries()
                .Where(x => x.BrandId == request.BrandId)
                .Where(x => x.ReportDate >= from && x.ReportDate <= to)
                .Where(tgsPlayer != null, x => x.PlayerCode == tgsPlayer.PlayerCode).ToArrayAsync(cancellationToken);

            var vipModels = playerAggregate
                .GroupBy(x => new
                {
                    x.ReportDate,
                    x.LiveDealerRebate,
                    x.CardGameRebate,
                    x.SlotRebate,
                    x.SportsRebate,
                    x.VirtualSportsRebate
                }, (x, players) =>
                    new VipRebateModel
                    {
                        ReportDate = x.ReportDate,
                        LiveDealerRebate = x.LiveDealerRebate,
                        LiveDealerAmount = players.Sum(m => m.LiveDealerAmount),
                        LiveDealerValidBetAmount = players.Sum(m => m.LiveDealerValidBetAmount),
                        SlotRebate = x.SlotRebate,
                        SlotAmount = players.Sum(m => m.SlotAmount),
                        SlotValidBetAmount = players.Sum(m => m.SlotValidBetAmount),
                        CardGameRebate = x.CardGameRebate,
                        CardGameAmount = players.Sum(m => m.CardGameAmount),
                        CardGameValidBetAmount = players.Sum(m => m.CardGameValidBetAmount),
                        SportsRebate = x.SportsRebate,
                        SportsAmount = players.Sum(m => m.SportsAmount),
                        SportsValidBetAmount = players.Sum(m => m.SportsValidBetAmount),
                        VirtualSportsRebate = x.VirtualSportsRebate,
                        VirtualSportsAmount = players.Sum(m => m.VirtualSportsAmount),
                        VirtualSportsValidBetAmount = players.Sum(m => m.VirtualSportsValidBetAmount)
                    });

            return await vipModels.CreatePagedResponseAsync(vipModels.Count());
        }
    }
}