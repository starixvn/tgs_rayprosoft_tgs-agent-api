using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Services.BackOffice.Application.Models.GameProvider;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Commands.GameProviderHandlers
{
    public class CreateGameProviderHandler : Handler<CreateGameProviderRequest, GameProviderResponse>
    {
        private readonly IGameProviderService _service;

        public CreateGameProviderHandler(IGameProviderService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<GameProviderResponse> Handle(CreateGameProviderRequest request,
            CancellationToken cancellationToken)
        {
            var games = request.Games.GroupBy(x => new {x.ExternalId, x.PlatformType}).Where(x => x.Count() > 1)
                .Select(x => x.Key.ExternalId).ToArray();

            if (games.Length > 0)
            {
                var response = CreateInvalidResponse<GameProviderResponse>(GameProvider.DuplicateGames);
                response.DuplicateGameExternalIds = games;
                return response;
            }

            if (await _service.ExistsAsync(x => request.Id != x.Id && x.Code == request.Code))
            {
                return CreateInvalidResponse<GameProviderResponse>(GameProvider.CodeExisted);
            }

            if (await _service.ExistsAsync(x => request.Id != x.Id && x.Name == request.Name))
            {
                return CreateInvalidResponse<GameProviderResponse>(GameProvider.NameExisted);
            }

            await _service.CreateGameProviderAsync(request);

            return CreateValidResponse<GameProviderResponse>();
        }
    }
}