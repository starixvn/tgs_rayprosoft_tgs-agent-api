using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Models.Extensions;
using TGS.Services.Operations;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class GetLobbyGameUrlQueryService : IGetService<GetLobbyGameUrlQuery, LobbyGameUrlInfo>
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGetService<GetGameUrlQuery, GameUrlInfo> _getGameUrlQueryService;

        public GetLobbyGameUrlQueryService(
            IGameProviderOperations gameProviderOperations,
            IGetService<GetGameUrlQuery, GameUrlInfo> getGameUrlQueryService)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _getGameUrlQueryService = getGameUrlQueryService ?? throw new ArgumentNullException(nameof(getGameUrlQueryService));
        }

        public async ValueTask<LobbyGameUrlInfo> GetAsync(GetLobbyGameUrlQuery query, CancellationToken cancellationToken = default)
        {
            var playerTokenData = query.PlayerTokenData;
            var platformType = playerTokenData.PlatformType;
            var gameProviderCode = query.GameProviderCode;
            var gameType = query.GameType;

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);

            var gamePlatformTypes = platformType.ToGamePlatformTypes();
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, gamePlatformTypes, gameType);
            if (games.Length != 1)
            {
                throw new InvalidOperationException(
                    $"Game Provider '{gameProviderCode}' with game type '{gameType}' and platform '{platformType}' should only have 1 lobby but found {games.Length}.");
            }

            var gameId = games[0].Id;

            var getGameUrlQuery = new GetGameUrlQuery(
                query.BrandId,
                playerTokenData,
                query.PlayerType,
                gameProviderCode,
                gameId,
                query.LanguageCode,
                query.Extends);
            var gameUrlInfo = await _getGameUrlQueryService
                .GetAsync(getGameUrlQuery)
                .ConfigureAwait(false);

            return new LobbyGameUrlInfo(
                gameUrlInfo.Error,
                gameUrlInfo.ErrorMessage,
                gameUrlInfo.Url);
        }
    }
}