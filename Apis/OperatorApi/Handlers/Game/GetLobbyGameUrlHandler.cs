using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operations;

namespace TGS.Apis.OperatorApi.Handlers.Game
{
    public sealed class GetLobbyGameUrlHandler : IRequestHandler<GetLobbyGameUrlRequest, GetGameUrlResponse>
    {
        private readonly HttpContext _httpContext;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IMediator _mediator;

        public GetLobbyGameUrlHandler(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IMediator mediator)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
            _mediator = mediator;
        }

        async Task<GetGameUrlResponse> IRequestHandler<GetLobbyGameUrlRequest, GetGameUrlResponse>.Handle(
            GetLobbyGameUrlRequest request, CancellationToken cancellationToken)
        {
            var playerTokenData = _httpContext.Items.GetPlayerTokenData();
            var platformType = playerTokenData.PlatformType;

            var gameProviderCode = request.GameProviderCode;
            var gameType = request.GameType;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);

            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, platformType, gameType);
            if (games.Length != 1)
            {
                throw new InvalidOperationException(
                    $"Game Provider '{gameProviderCode}' with game type '{gameType}' and platform '{platformType}' should only have 1 lobby but found {games.Length}.");
            }

            var gameId = games[0].Id;
            var r = new GetGameUrlRequest
            {
                GameProviderCode = gameProviderCode,
                GameId = gameId,
                LanguageCode = request.LanguageCode
            };

            return await _mediator.Send(r, cancellationToken);
        }
    }
}