using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Round;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.ReportHandlers.Round
{
    internal class GetRoundsQueryHandler : Handler<GetRoundsRequest, PagedResponse<RoundModel>>
    {
        private readonly IRoundService _service;

        public GetRoundsQueryHandler(IRoundService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        //TODO: calculate every column amount
        public override Task<PagedResponse<RoundModel>> Handle(
            GetRoundsRequest request,
            CancellationToken cancellationToken)
        {
            return _service.GetRoundsAsync()
                .Where(x => x.BrandId == request.BrandId)
                .Where(x => x.ExternalClosedOn >= request.From && x.ExternalClosedOn <= request.To)
                .Where(request.GameProviderId, x => request.GameProviderId == x.GameProviderId)
                .Where(request.Status, x => request.Status == x.Status)
                .Join(_service.Repository.Players.Where(request.PlayerName, x => request.PlayerName == x.PlayerCode),
                    round => round.PlayerCode, player => player.PlayerCode, (round, player) => new {round, player})
                .Join(_service.Repository.Games, combine => combine.round.GameId, game => game.Id,
                    (combine, game) => new {combine.round, combine.player, game})
                .Join(_service.Repository.RoundDetails, c => c.round.Id, detail => detail.RoundId,
                    (combine, detail) => new {combine.round, combine.player, combine.game, detail})
                .Join(_service.Repository.GameProviders, combine => combine.round.GameProviderId,
                    gameProvider => gameProvider.Id, (combine, gameProvider) => new RoundModel
                    {
                        Id = combine.round.Id,
                        PlayerName = combine.player.RealName,
                        GameProviderId = combine.round.GameProviderId,
                        GameProviderName = gameProvider.Name,
                        ExternalRoundId = combine.round.ExternalRoundId,
                        CreatedOn = combine.round.CreatedOn,
                        ClosedOn = combine.round.ClosedOn,
                        CurrencyCode = combine.round.CurrencyCode,
                        BetAmount = combine.round.BetAmount,
                        ValidBetAmount = combine.round.BetAmount,
                        WinLoss = combine.round.WinLoss,
                        AdditionalData = combine.detail.AdditionalData
                    })
                .CreatePagedResponseAsync(request);
        }
    }
}