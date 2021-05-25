using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Services.BackOffice.Application.Models.Game;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.GameHandlers
{
    public class CreateGamesHandler : Handler<CreateGamesRequest, CreateGamesResponse>
    {
        private readonly IGameService _gameService;
        private readonly IGameProviderService _service;

        public CreateGamesHandler(IGameProviderService service, IGameService gameService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
            _gameService = gameService;
        }

        public override async Task<CreateGamesResponse> Handle(CreateGamesRequest request,
            CancellationToken cancellationToken)
        {
            var gameProvider = await _service.GetGameProviderAsync(x => x.Id == request.GameProviderId);

            var gameCodesExisted = gameProvider.Games.Select(x => x.Code).Intersect(request.Games.Select(x => x.Code))
                .ToArray();
            var gameExternalIdsExisted = gameProvider.Games.Select(x => x.ExternalId)
                .Intersect(request.Games.Select(x => x.ExternalId)).ToArray();

            var games = request.Games.GroupBy(x => x.ExternalId).Where(x => x.Count() > 1)
                .Select(x => x.Key).ToArray();

            if (games.Length > 0)
            {
                var response = CreateInvalidResponse<CreateGamesResponse>(GameProvider.DuplicateGames);
                response.DuplicateIds = games;
                return response;
            }

            if (gameExternalIdsExisted.Length > 0)
            {
                var response = CreateInvalidResponse<CreateGamesResponse>(GameProvider.ExternalIdExisted);
                response.DuplicateIds = gameExternalIdsExisted;
                return response;
            }

            if (gameCodesExisted.Length > 0)
            {
                var response = CreateInvalidResponse<CreateGamesResponse>(GameProvider.CodeExisted);
                response.DuplicateIds = gameCodesExisted;
                return response;
            }

            await _gameService.CreateGamesAsync(gameProvider, request);

            return CreateValidResponse<CreateGamesResponse>();
        }
    }
}