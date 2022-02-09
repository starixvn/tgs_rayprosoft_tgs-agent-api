using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Services.Report.Application.Models.GameProvider;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.GameProvider
{
    internal class
        GetGameProviderRoundsQueryHandler : Handler<GetGameProviderRoundsRequest,
            GetGameProviderRoundsResponse>
    {
        private readonly IPlayerGameAggregateService _service;
        private readonly ITgsRoundService _tgsRoundService;

        public GetGameProviderRoundsQueryHandler(IPlayerGameAggregateService service, ITgsRoundService tgsRoundService,
            IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
            _tgsRoundService = tgsRoundService;
        }

        public override async Task<GetGameProviderRoundsResponse> Handle(
            GetGameProviderRoundsRequest request,
            CancellationToken cancellationToken)
        {
            if ((request.From is null || request.To is null) &&
                (request.ExternalFrom is null || request.ExternalTo is null))
            {
                return new GetGameProviderRoundsResponse();
            }

            var transactionCounts = await _tgsRoundService.GetRoundQueries()
                .Where(x => x.BrandId == request.BrandId)
                .Where(request.From is not null && request.To is not null,
                    x => x.ClosedOn >= request.From && x.ClosedOn < request.To)
                .Where(request.ExternalFrom is not null && request.ExternalTo is not null,
                    x => x.ExternalClosedOn >= request.ExternalFrom && x.ExternalClosedOn < request.ExternalTo)
                .Join(_tgsRoundService.TgsRepository.Games,
                    t => t.GameId,
                    g => g.Id,
                    (round, game) => new
                    {
                        round.GameProviderId,
                        round.GameId,
                        round.CurrencyCode,
                        game.Type,
                        game.GameProvider.Code
                    })
                .Where(request.GameProviderCode, x => x.Code == request.GameProviderCode)
                .Where(request.GameType is not null, x => x.Type == request.GameType)
                .GroupBy(x => new
                    {
                        x.GameProviderId,
                        x.Type,
                        x.GameId,
                        x.CurrencyCode
                    },
                    (group, items) => new
                    {
                        group.GameProviderId,
                        group.Type,
                        group.GameId,
                        group.CurrencyCode,
                        Count = items.Count()
                    }).ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var response = await _service.GetPlayerGameAggregateQueries()
                .Where(x => x.ReportDate >= request.From && x.ReportDate < request.To)
                .Where(x => x.BrandId == request.BrandId)
                .Where(request.GameType is not null, x => x.GameType == request.GameType)
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
                .Where(request.GameProviderCode, x => x.gp.Code == request.GameProviderCode)
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
                        GameProviderId = group.GameProviderId,
                        GameProviderName = group.Name,
                        CurrencyCode = group.CurrencyCode,
                        BetAmount = items.Sum(i => i.aggregate.BetAmount),
                        ValidBetAmount = items.Sum(i => i.aggregate.ValidBetAmount),
                        OriginalTurnover = items.Sum(i => i.aggregate.OriginalTurnover),
                        CompanyWinLoss = items.Sum(i => i.aggregate.WinLoss)
                    }).ToArrayAsync(cancellationToken);

            foreach (var item in response)
            {
                item.TransactionCount =
                    transactionCounts
                        .Where(t => t.GameProviderId == item.GameProviderId && t.CurrencyCode == item.CurrencyCode)
                        .Sum(t => t.Count);
            }

            return new GetGameProviderRoundsResponse
            {
                Datas = response.AsQueryable().CreatePagedResponse(request),
                Totals = response.GroupBy(x => new { x.CurrencyCode },
                    (group, items) => new GameProviderRoundTotalModel
                    {
                        CurrencyCode = group.CurrencyCode,
                        TransactionCount =
                            transactionCounts
                                .Where(x => x.CurrencyCode == group.CurrencyCode).Sum(x => x.Count),
                        BetAmount = items.Sum(i => i.BetAmount),
                        ValidBetAmount = items.Sum(i => i.ValidBetAmount),
                        OriginalTurnover = items.Sum(i => i.OriginalTurnover),
                        CompanyWinLoss = items.Sum(i => i.CompanyWinLoss)
                    }).ToArray()
            };
        }
    }
}