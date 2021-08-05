using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operator.GameProviders;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class GetAllGamesByGameTypeQueryService : IQueryService<GetAllGamesByGameTypeQuery, GameInfo>
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IValidStatus<Status> _validStatus;

        public GetAllGamesByGameTypeQueryService(
            IBrandOperations brandOperations,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus,
            IGameProviderRepository gameProviderRepository,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _validStatus = validStatus ?? throw new ArgumentNullException(nameof(validStatus));
            _validGameStatus = validGameStatus ?? throw new ArgumentNullException(nameof(validGameStatus));
            _gameProviderRepository =
                gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _jsonSerializationProvider = jsonSerializationProvider ??
                throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        public async IAsyncEnumerable<GameInfo> QueryAsync(
            GetAllGamesByGameTypeQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var gamePlatformTypes = query.PlatformType.ToGamePlatformTypes();
            var statuses = _validStatus.GetStatus(query.PlayerType);
            var gameStatuses = _validGameStatus.GetStatus(query.PlayerType);

            var brandGameProviders = await _brandOperations
                .GetBrandGameProvidersAsync(query.BrandId)
                .ConfigureAwait(false);

            var brandGames = await _brandOperations
                .GetBrandGamesAsync(query.BrandId)
                .ConfigureAwait(false);

            var subGps = await _gameProviderRepository
                .QuerySubGameProvidersAsync()
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            var gameProviderIds = (from bg in brandGames
                                   where gamePlatformTypes.Contains(bg.PlatformType)
                                   from subGp in subGps
                                   where bg.GameProviderId == subGp.GameProviderId
                                   select bg.GameProviderId).Distinct().ToArray();

            var gpStatuses = new[] {
                Status.Active,
                Status.Maintenance
            };

            var gameProviders = await _gameProviderRepository
                .QueryGameProvidersAsync()
                .Where(gp => gameProviderIds.Contains(gp.Id)
                    && gpStatuses.Contains(gp.Status))
                .ToArrayAsync(cancellationToken)
                .ConfigureAwait(false);

            var games = await _gameProviderRepository.QueryGamesAsync(
                    gameProviderIds,
                    new[] {
                        GameStatus.Active,
                        GameStatus.Testing,
                        GameStatus.Maintenance
                    },
                    gamePlatformTypes)
                .ToArrayAsync(cancellationToken).ConfigureAwait(true);

            var gameTypes = query.Types;
            var infos = from g in games
                        join bg in brandGames on g.Id equals bg.GameId
                        let gp = gameProviders.First(gp => gp.Id == bg.GameProviderId)
                        let bgp = brandGameProviders.First(bgp => bgp.GameProviderId == bg.GameProviderId)
                        where g.AvailableToBrand
                            && (gameTypes.Length == 0
                                || gameTypes.Contains(g.Type))
                        let isActive = statuses.Contains(gp.Status)
                            && statuses.Contains(bgp.BrandGameProviderStatus)
                            && gameStatuses.Contains(bg.BrandGameStatus)
                            && gameStatuses.Contains(g.Status)
                        select GetGameModels(g, bg, isActive);

            foreach (var item in infos.OrderBy(g => g.Id))
            {
                yield return item;

                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
            }
        }

        private GameInfo GetGameModels(
            GameDataInfo game,
            CachedBrandGame brandGame,
            bool isActive)
        {
            var info = new GameInfo
            {
                Id = game.Id,
                Code = game.Code,
                Name = game.Name,
                Description = game.Description,
                GameProviderCode = game.SubGameProviderCode,
                Type = game.Type,
                IsActive = isActive,
                Tag = new TagInfo
                {
                    New = brandGame.Tag.IsNew(),
                    Hot = brandGame.Tag.IsHot(),
                    Recommend = brandGame.Tag.IsRecommend()
                },
                DisplayOrder = brandGame.DisplayOrder,
                Localization = string.IsNullOrEmpty(game.Localization)
                    ? new Dictionary<string, JsonDocument>()
                    : _jsonSerializationProvider.DeserializeFromString<Dictionary<string, JsonDocument>>(
                        game.Localization)
            };

            return info;
        }
    }
}