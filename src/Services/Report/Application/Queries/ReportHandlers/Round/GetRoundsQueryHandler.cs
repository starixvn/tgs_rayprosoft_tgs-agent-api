using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Report.Application.Models.Round;
using TGS.Services.Report.Domain.Enums;
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
            CancellationToken cancellationToken) =>
            _service.GetRoundsAsync()
                .Where(x => x.BrandId == request.BrandId)
                .Where(x => x.CreatedOn >= request.From && x.CreatedOn <= request.To)
                .Where(request.GameProviderId, x => request.GameProviderId == x.GameProviderId)
                .Where(request.Status, x => request.Status == x.Status)
                .Where(request.RoundId, x => x.Id == request.RoundId)
                .GroupJoin(
                    _service.Repository.Players,
                    round => round.PlayerCode, player => player.PlayerCode,
                    (round, player) => new {round, player})
                .SelectMany(x => x.player.DefaultIfEmpty(), (x, player) => new {x.round, player})
                .GroupJoin(_service.Repository.Games,
                    combine => combine.round.GameId, game => game.Id,
                    (combine, game) => new {combine.round, combine.player, game})
                .SelectMany(x => x.game.DefaultIfEmpty(), (x, game) => new {x.round, x.player, game})
                .GroupJoin(_service.Repository.RoundDetails, c => c.round.Id, detail => detail.RoundId,
                    (combine, detail) => new {combine.round, combine.player, combine.game, detail})
                .SelectMany(x => x.detail.DefaultIfEmpty(), (x, detail) => new {x.round, x.player, x.game, detail})
                .Where(request.Username, x => request.Username == x.player.Username)
                .Where(request.GameType, x => x.game.Type == request.GameType)
                .GroupJoin(_service.Repository.GameProviders, combine => combine.round.GameProviderId,
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
                .Select(combine => new RoundModel
                {
                    Id = combine.round.Id,
                    Status =
                        combine.round.Status == RoundStatus.Closed && -combine.round.WinLoss > 0
                            ? ReportRoundStatus.Win
                            : combine.round.Status == RoundStatus.Closed && -combine.round.WinLoss < 0
                                ? ReportRoundStatus.Loss
                                : combine.round.Status == RoundStatus.Closed && -combine.round.WinLoss == 0
                                    ? ReportRoundStatus.Draw
                                    : ReportRoundStatus.Open,
                    PlayerCode = combine.player.PlayerCode,
                    ExternalPlayerCode = combine.player.PlayerCode,
                    PlayerName = combine.player.Username,
                    GameProviderId = combine.round.GameProviderId,
                    GameProviderCode = combine.game.GameProvider.Code,
                    GameProviderName = combine.gameProvider.Name,
                    GameId = combine.game.Id,
                    GameName = combine.game.Name,
                    ExternalRoundId = combine.round.ExternalRoundId,
                    ExternalCreatedOn = combine.round.ExternalCreatedOn,
                    ExternalClosedOn = combine.round.ExternalClosedOn,
                    CreatedOn = combine.round.CreatedOn,
                    ClosedOn = combine.round.ClosedOn,
                    CurrencyCode = combine.round.CurrencyCode,
                    BetAmount = combine.round.BetAmount,
                    ValidBetAmount = combine.round.Turnover,
                    SettleAmount = combine.round.SettleAmount,
                    WinLoss = -combine.round.WinLoss,
                    AdditionalData = combine.detail.AdditionalData
                })
                .CreatePagedResponseAsync(request);
    }
}