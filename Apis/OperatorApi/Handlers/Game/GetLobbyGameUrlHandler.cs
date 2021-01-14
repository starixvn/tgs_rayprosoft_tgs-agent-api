using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operations;

namespace TGS.Apis.OperatorApi.Handlers.Game
{
    public sealed class GetLobbyGameUrlHandler : IRequestHandler<GetLobbyGameUrlRequest, GetGameUrlResponse>
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IMediator _mediator;

        public GetLobbyGameUrlHandler(
            IGameProviderOperations gameProviderOperations,
            IMediator mediator)
        {
            _gameProviderOperations = gameProviderOperations;
            _mediator = mediator;
        }

        async Task<GetGameUrlResponse> IRequestHandler<GetLobbyGameUrlRequest, GetGameUrlResponse>.Handle(
            GetLobbyGameUrlRequest request, CancellationToken cancellationToken)
        {
            var gameProviderCode = request.GameProviderCode;
            var gameType = request.GameType;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);

            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, gameType);
            if (games.Length != 1)
            {
                throw new InvalidOperationException(
                    $"Game Provider '{gameProviderCode}' with game type '{gameType}' should only have 1 lobby but found {games.Length}.");
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