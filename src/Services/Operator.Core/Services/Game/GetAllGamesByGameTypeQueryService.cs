using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Services.Operator.GameProviders;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class GetAllGamesByGameTypeQueryService : IQueryService<GetAllGamesByGameTypeQuery, GameInfo>
    {
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IValidStatus<Status> _validStatus;

        public GetAllGamesByGameTypeQueryService(
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus,
            IGameProviderRepository gameProviderRepository)
        {
            _validStatus = validStatus ?? throw new ArgumentNullException(nameof(validStatus));
            _validGameStatus = validGameStatus ?? throw new ArgumentNullException(nameof(validGameStatus));

            _gameProviderRepository =
                gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public async IAsyncEnumerable<GameInfo> QueryAsync(
            GetAllGamesByGameTypeQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var gamePlatformTypes = query.PlatformType.ToGamePlatformTypes();
            var statuses = _validStatus.GetStatus(query.PlayerType);
            var gameStatuses = _validGameStatus.GetStatus(query.PlayerType);

            var gpStatuses = new[]
            {
                Status.Active,
                Status.Maintenance
            };

            var brandSubGps = await _gameProviderRepository.QueryAllSubGameProvidersAsync(query.BrandId)
                .Where(subGp => gpStatuses.Contains(subGp.GpStatus) && gpStatuses.Contains(subGp.BrandGpStatus))
                .ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var brandGames = await _gameProviderRepository.QueryAllGamesAsync(query.BrandId)
                .Where(g => gamePlatformTypes.Contains(g.PlatformType))
                .ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var gameTypes = query.Types;

            var infos = from g in brandGames
                        join sgp in brandSubGps on g.GpId equals sgp.GameProviderId
                        where g.AvailableToBrand && (gameTypes.Length == 0
                            || gameTypes.Contains(g.Type))
                        let isActive = statuses.Contains(sgp.GpStatus)
                            && statuses.Contains(sgp.BrandGpStatus)
                            && gameStatuses.Contains(g.BrandGameStatus)
                            && gameStatuses.Contains(g.GameStatus)
                        select new GameInfo
                        {
                            Id = g.Id,
                            Code = g.Code,
                            Name = g.Name,
                            Description = g.Description,
                            GameProviderCode = g.SubGpCode,
                            Type = g.Type,
                            IsActive = isActive,
                            Tag = new TagInfo
                            {
                                New = g.Tag.IsNew(),
                                Hot = g.Tag.IsHot(),
                                Recommend = g.Tag.IsRecommend()
                            },
                            DisplayOrder = g.DisplayOrder,
                            Localization = string.IsNullOrEmpty(g.Localization)
                                ? new Dictionary<string, JsonDocument>()
                                : JsonSerializer.Deserialize<Dictionary<string, JsonDocument>>(
                                    g.Localization)
                        };

            foreach (var item in infos.OrderBy(g => g.Id))
            {
                yield return item;

                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
            }
        }
    }
}