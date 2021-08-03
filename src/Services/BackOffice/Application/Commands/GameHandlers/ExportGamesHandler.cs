using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ClosedXML.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Infrastructures.Utility;
using TGS.Services.BackOffice.Application.Models.Game;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.GameHandlers
{
    public class ExportGamesHandler : Handler<ExportGamesRequest, HttpResponseMessage>
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly IGameService _gameService;

        public ExportGamesHandler(IGameService gameService, IGameProviderService gameProviderService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _gameService = gameService;
            _gameProviderService = gameProviderService;
        }

        public override async Task<HttpResponseMessage> Handle(ExportGamesRequest request,
            CancellationToken cancellationToken)
        {
            var games = await _gameService.GetGameQueries(q => q.Where(x => x.GameProviderId == request.Id))
                .Select(x => new ExportGameModel
                {
                    Name = x.Name,
                    Code = x.Code,
                    RuleUrl = x.RuleUrl,
                    ExternalId = x.ExternalId,
                    Type = x.Type.ToString(),
                    PlatformType = x.PlatformType.ToString(),
                    HasTurnoverOnTieBet = x.HasTurnoverOnTieBet,
                    EndpointPath = x.EndpointPath,
                    Description = x.Description,
                    SubGame = x.SubGame,
                    AvailableToBrand = x.AvailableToBrand,
                    Html5 = x.Html5,
                    AvailableCurrencies = x.AvailableCurrencies,
                    SubGpCode = x.SubGameProviderCode
                })
                .ToArrayAsync(cancellationToken);

            var xlsx = games.Export("games");

            return xlsx.Deliver("Games.xlsx");
        }
    }
}