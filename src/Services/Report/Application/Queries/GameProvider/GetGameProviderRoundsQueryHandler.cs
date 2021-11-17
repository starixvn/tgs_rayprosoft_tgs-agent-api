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
                .Join(_service.Repository.Brands,
                    group => group.aggregate.BrandId,
                    brand => brand.Id,
                    (group, brand) => new
                    {
                        group.aggregate,
                        group.gp,
                        brand
                    })
                .GroupBy(x => new
                    {
                        x.aggregate.GameProviderId,
                        x.gp.Name,
                        x.brand.Currencies.First().CurrencyCode
                    },
                    (group, items) => new
                    {
                        group.GameProviderId,
                        GameProviderName = group.Name,
                        group.CurrencyCode,
                        items
                    })
                .Select(x => new GameProviderRoundsModel
                {
                    GameProviderName = x.GameProviderName,
                    CurrencyCode = x.CurrencyCode,
                    BetAmount = x.items.Sum(i => i.aggregate.BetAmount),
                    ValidBetAmount = x.items.Sum(i => i.aggregate.ValidBetAmount),
                    Turnover = x.items.Sum(i => i.aggregate.Turnover),
                    CompanyWinLoss = x.items.Sum(i => i.aggregate.WinLoss)
                }).CreatePagedResponseAsync(request);
        }
    }
}