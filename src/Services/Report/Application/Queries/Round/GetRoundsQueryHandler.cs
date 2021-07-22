using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Round;
using TGS.Services.Report.Infrastructure.DataService;

namespace TGS.Services.Report.Application.Queries.Round
{
    public class GetRoundsQueryHandler : Handler<GetRoundsRequest, PagedResponse<RoundModel>>
    {
        private readonly ITgsRoundService _service;

        public GetRoundsQueryHandler(ITgsRoundService service, IHttpContextAccessor accessor) :
            base(accessor)
        {
            _service = service;
        }

        public override Task<PagedResponse<RoundModel>> Handle(
            GetRoundsRequest request,
            CancellationToken cancellationToken) =>
            _service.GetRoundQueries(q => q.Where(x => x.BrandId == request.BrandId)
                    .Where(x => x.CreatedOn >= request.From && x.CreatedOn <= request.To)
                    .Where(request.GameProviderId, x => request.GameProviderId == x.GameProviderId)
                    .Where(request.Status, x => request.Status == x.ClientStatus)
                    .Where(request.RoundId, x => x.Id == request.RoundId)
                    .Where(request.PlayerCode, x => x.PlayerCode == request.PlayerCode)
                )
                .GroupJoin(
                    _service.TgsRepository.Players,
                    round => round.PlayerCode, player => player.PlayerCode,
                    (round, player) => new {round, player})
                .SelectMany(x => x.player.DefaultIfEmpty(), (x, player) => new {x.round, player})
                .GroupJoin(_service.TgsRepository.Games,
                    combine => combine.round.GameId, game => game.Id,
                    (combine, game) => new {combine.round, combine.player, game})
                .SelectMany(x => x.game.DefaultIfEmpty(), (x, game) => new {x.round, x.player, game})
                .GroupJoin(_service.TgsRepository.RoundDetails, c => c.round.Id, detail => detail.RoundId,
                    (combine, detail) => new {combine.round, combine.player, combine.game, detail})
                .SelectMany(x => x.detail.DefaultIfEmpty(), (x, detail) => new {x.round, x.player, x.game, detail})
                .Where(request.Username, x => request.Username == x.player.Username)
                .Where(request.GameType, x => x.game.Type == request.GameType)
                .GroupJoin(_service.TgsRepository.GameProviders, combine => combine.round.GameProviderId,
                    gameProvider => gameProvider.Id,
                    (combine, gameProvider) => new
                    {
                        combine.round,
                        combine.player,
                        combine.game,
                        combine.detail,
                        gameProvider
                    })
                .SelectMany(x => x.gameProvider.DefaultIfEmpty(),
                    (x, gameProvider) => new
                    {
                        x.round,
                        x.player,
                        x.game,
                        x.detail,
                        gameProvider
                    })
                .GroupJoin(_service.TgsRepository.BrandGames,
                    roundGroup => new {roundGroup.round.BrandId, roundGroup.round.GameId},
                    brandGame => new {brandGame.BrandId, brandGame.GameId},
                    (roundGroup, brandGame) => new
                    {
                        roundGroup.round,
                        roundGroup.player,
                        roundGroup.game,
                        roundGroup.gameProvider,
                        roundGroup.detail,
                        brandGame
                    })
                .SelectMany(x => x.brandGame.DefaultIfEmpty(),
                    (x, brandGame) => new
                    {
                        x.round,
                        x.player,
                        x.game,
                        x.gameProvider,
                        x.detail,
                        brandGame
                    })
                .Select(combine => new RoundModel
                {
                    Id = combine.round.Id,
                    Status = combine.round.ClientStatus,
                    PlayerCode = combine.player.PlayerCode,
                    ExternalPlayerCode = combine.player.PlayerCode,
                    PlayerName = combine.player.Username,
                    GameProviderId = combine.round.GameProviderId,
                    GameProviderCode = combine.game.GameProvider.Code,
                    GameProviderName = combine.gameProvider.Name,
                    SubGameProviderName =combine.game.SubGameProvider.Name,
                    GameId = combine.game.Id,
                    GameName = combine.game.Name,
                    ExternalRoundId = combine.round.ExternalRoundId,
                    ExternalCreatedOn = combine.round.ExternalCreatedOn,
                    ExternalClosedOn = combine.round.ExternalClosedOn,
                    CreatedOn = combine.round.CreatedOn,
                    ClosedOn = combine.round.ClosedOn,
                    CurrencyCode = combine.round.CurrencyCode,
                    BetAmount = combine.round.BetAmount,
                    ValidBetAmount =
                        combine.round.Turnover * (combine.round.EffectiveBettingPercentage ??
                            (combine.brandGame == null ? 100M : combine.brandGame.EffectiveBettingPercentage)) / 100M,
                    SettleAmount = combine.round.SettleAmount,
                    WinLoss = -combine.round.WinLoss,
                    AdditionalData = combine.detail.AdditionalData
                })
                .OrderByDescending(x => x.CreatedOn)
                .CreatePagedResponseAsync(request);
    }
}