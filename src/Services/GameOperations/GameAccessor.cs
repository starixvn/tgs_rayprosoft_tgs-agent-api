using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Models.Enums;
using TGS.Services.GameOperations.Models.Data;
using TGS.Services.Operations;

namespace TGS.Services.GameOperations
{
    internal class GameAccessor : IGameAccessor
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;

        public GameAccessor(
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations)
        {
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }

        public async ValueTask<IEnumerable<GameInfo>> GetBrandAvailableGamesAsync(
            Guid brandId,
            Guid gameProviderId,
            IEnumerable<string> gameExternalIds,
            IEnumerable<GamePlatformType> gamePlatformTypes)
        {
            var externalIds = gameExternalIds.ToArray();
            if (externalIds.Length == 0)
            {
                throw new InvalidOperationException("No expected game id is found when getting brand available games");
            }

            var platformTypes = gamePlatformTypes.ToArray();
            if (platformTypes.Length == 0)
            {
                throw new InvalidOperationException("No expected game platform type is found when getting brand available games");
            }

            var brandGames =
                await _brandOperations.GetBrandGamesAsync(brandId, gameProviderId, platformTypes)
                    .ConfigureAwait(false);

            var cachedGames = await _gameProviderOperations
                .GetCachedGamesAsync(gameProviderId, platformTypes)
                .ConfigureAwait(false);

            var games = cachedGames
                .Where(g => externalIds.Contains(g.ExternalId))
                .Select(game =>
                {
                    var brandGame = brandGames.FirstOrDefault(bg => bg.GameId == game.Id);
                    var status = brandGame != null ?
                        GetGameStatus(brandGame.BrandGameStatus, game.Status)
                        : game.Status;

                    return new GameInfo(game.Id, game.ExternalId, status);
                }).ToArray();

            if (games.Length == 0)
            {
                throw new InvalidOperationException(
                    $"Brand '{brandId}' doesn't have games '{string.Join(",", externalIds)}'");
            }

            return games;
        }

        private GameStatus GetGameStatus(GameStatus brandGameStatus, GameStatus gameStatus)
        {
            if (brandGameStatus == GameStatus.Inactive || gameStatus == GameStatus.Inactive)
            {
                return GameStatus.Inactive;
            }

            if (brandGameStatus == GameStatus.Testing || gameStatus == GameStatus.Testing)
            {
                return GameStatus.Testing;
            }

            if (brandGameStatus == GameStatus.Maintenance || gameStatus == GameStatus.Maintenance)
            {
                return GameStatus.Maintenance;
            }

            return GameStatus.Active;
        }
    }
}
