using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Enums;
using TGS.Services.BackOffice.Application.Models.GameProvider;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;
using TGS.Services.BackOffice.Infrastructure.Services;

namespace TGS.Services.BackOffice.Application.Commands.GameProviderHandlers
{
    public class UploadGamesHandler : Handler<UploadGamesRequest, UploadGamesResponse>
    {
        private readonly IGameProviderService _gameProviderService;
        private readonly IUploadService _uploadService;

        public UploadGamesHandler(IUploadService uploadService, IGameProviderService gameProviderService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _uploadService = uploadService;
            _gameProviderService = gameProviderService;
        }

        public override async Task<UploadGamesResponse> Handle(UploadGamesRequest request,
            CancellationToken cancellationToken)
        {
            var gameProvider = request.Id != null
                ? await _gameProviderService.GetGameProviderAsync(x => x.Id == request.Id)
                : null;
            var uploadedResult = await _uploadService.GetUploadedExcelAsync<UploadGameModel>(request.File);

            var existedGames = uploadedResult
                .Where(x => gameProvider?.Games.Any(g =>
                    g.ExternalId == x.ExternalId &&
                    Enum.Parse(typeof(PlatformType), x.Platform, true).Equals(g.PlatformType)) ?? false).ToArray();

            return existedGames.Any()
                ? new UploadGamesResponse
                {
                    Valid = false,
                    Message = GameProvider.GamesExists,
                    Games = existedGames.ToArray()
                }
                : new UploadGamesResponse
                {
                    Valid = true,
                    Games = uploadedResult.ToArray()
                };
        }
    }
}