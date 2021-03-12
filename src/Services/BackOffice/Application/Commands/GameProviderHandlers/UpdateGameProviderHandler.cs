using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.GameProvider;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.GameProviderHandlers
{
    public class UpdateGameProviderHandler : Handler<UpdateGameProviderRequest, GameProviderResponse>
    {
        private readonly IBrandGameService _brandGameService;
        private readonly IGameProviderService _service;

        public UpdateGameProviderHandler(IGameProviderService service, IBrandGameService brandGameService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
            _brandGameService = brandGameService;
        }

        public override async Task<GameProviderResponse> Handle(UpdateGameProviderRequest request,
            CancellationToken cancellationToken)
        {
            var gameProvider = await _service.GetGameProviderAsync(x => x.Id == request.Id);

            if (gameProvider == null)
            {
                return CreateInvalidResponse<GameProviderResponse>(GameProvider.NotFound);
            }

            if (await _service.ExistsAsync(x => request.Id != x.Id && x.Name == request.Name))
            {
                return CreateInvalidResponse<GameProviderResponse>(GameProvider.NameExisted);
            }

            if (await _service.ExistsAsync(x => request.Id != x.Id && x.Code == request.Code))
            {
                return CreateInvalidResponse<GameProviderResponse>(GameProvider.CodeExisted);
            }

            var games = request.Games.GroupBy(x => new {x.ExternalId, x.PlatformType}).Where(x => x.Count() > 1)
                .Select(x => x.Key.ExternalId).ToArray();

            if (games.Length > 0)
            {
                var response = CreateInvalidResponse<GameProviderResponse>(GameProvider.DuplicateGames);
                response.DuplicateGameExternalIds = games;
                return response;
            }

            var ids = request.Games.Where(g => g.Status != GameStatus.Active).Select(g => g.Id).ToArray();
            if (await _brandGameService.GetBrandGameQueries()
                .AnyAsync(
                    x =>
                        ids.Contains(x.GameId),
                    cancellationToken))
            {
                var response = CreateInvalidResponse<GameProviderResponse>(GameProvider.GameInUse);
                response.InUseGameIds = ids;
                return response;
            }

            await _service.UpdateGameProviderAsync(gameProvider, request);

            return CreateValidResponse<GameProviderResponse>();
        }
    }
}