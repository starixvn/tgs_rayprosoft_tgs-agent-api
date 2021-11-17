using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.GameProvider;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.GameProvider
{
    internal class
        GetGameProviderRoundsQueryHandler : Handler<GetGameProviderRoundsRequest,
            PagedResponse<GameProviderRoundsModel>>
    {
        private readonly IPlayerGameAggregateService _service;

        public GetGameProviderRoundsQueryHandler(IPlayerGameAggregateService service, IHttpContextAccessor accessor) :
            base(accessor)
            => _service = service;

        public override async Task<PagedResponse<GameProviderRoundsModel>> Handle(
            GetGameProviderRoundsRequest request,
            CancellationToken cancellationToken)
        {
            var gameProviderIds = await _service.Repository.GameProviders.AsQueryable()
                .Where(x => request.GameProviderCodes.Contains(x.Code))
                .Select(x => x.Id)
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            return await _service.GetPlayerGameAggregateQueries()
                .Where(x => x.ReportDate >= request.From && x.ReportDate < request.To)
                .Where(gameProviderIds.Length > 0, x => gameProviderIds.Contains(x.GameProviderId))
                .Where(x => x.BrandId == request.BrandId)
                .Where(x => x.GameType == request.GameType)
                .Join(_service.Repository.GameProviders,
                    aggregate => aggregate.GameProviderId,
                    gp => gp.Id,
                    (aggregate, gp) => new
                    {
                        aggregate,
                        gp
                    })
                .Select(x => new
                {
                    x.aggregate,
                    x.gp
                })
                .Join(_service.Repository.Brands,
                    group => group.aggregate.BrandId,
                    brand => brand.Id,
                    (group, brand) => new
                    {
                        group.aggregate,
                        group.gp,
                        brand
                    })
                .Select(x => new
                {
                    x.aggregate,
                    x.gp,
                    x.brand.Currencies.FirstOrDefault().CurrencyCode
                })
                .GroupBy(x => new
                    {
                        x.aggregate.GameProviderId,
                        x.gp.Name,
                        x.CurrencyCode
                    },
                    (group, items) => new GameProviderRoundsModel
                    {
                        GameProviderName = group.Name,
                        CurrencyCode = group.CurrencyCode,
                        BetAmount = items.Sum(i => i.aggregate.BetAmount),
                        ValidBetAmount = items.Sum(i => i.aggregate.ValidBetAmount),
                        OriginalTurnover = items.Sum(i => i.aggregate.OriginalTurnover),
                        CompanyWinLoss = items.Sum(i => i.aggregate.WinLoss)
                    }).CreatePagedResponseAsync(request);
        }
    }
}