using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Queries;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class GetGamesQueryService : IQueryService<GetGamesQuery, GameInfo>
    {
        private readonly IBrandCurrencyCodeMapper _brandCurrencyCodeMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandLanguageCodeMapper _brandLanguageCodeMapper;
        private readonly IGameQueries _gameQueries;
        private readonly IValidStatus<Status> _validStatus;
        private readonly IValidStatus<GameStatus> _validGameStatus;

        public GetGamesQueryService(
            IBrandCurrencyCodeMapper brandCurrencyCodeMapper,
            IGameProviderOperations gameProviderOperations,
            IBrandOperations brandOperations,
            IPlayerOperations playerOperations,
            IBrandLanguageCodeMapper brandLanguageCodeMapper,
            IGameQueries gameQueries,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus
            )
        {
            _brandCurrencyCodeMapper = brandCurrencyCodeMapper ?? throw new System.ArgumentNullException(nameof(brandCurrencyCodeMapper));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandLanguageCodeMapper = brandLanguageCodeMapper ?? throw new ArgumentNullException(nameof(brandLanguageCodeMapper));
            _gameQueries = gameQueries ?? throw new ArgumentNullException(nameof(gameQueries));
            _validStatus = validStatus ?? throw new ArgumentNullException(nameof(validStatus));
            _validGameStatus = validGameStatus ?? throw new ArgumentNullException(nameof(validGameStatus));
        }

        public async IAsyncEnumerable<GameInfo> QueryAsync(
            GetGamesQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var playerTokenData = query.PlayerTokenData;
            var currencyCode = await _brandCurrencyCodeMapper.ExternalToInternalAsync(query.CurrencyCode, query.BrandId);

            Guid[] gameProviderIds;
            if (!string.IsNullOrEmpty(query.GameProviderCode))
            {
                var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(query.GameProviderCode);
                gameProviderIds = new Guid[] { gameProviderId };
            }
            else
            {
                gameProviderIds = (await _brandOperations.GetBrandGameProvidersAsync(query.BrandId))
                    .Select(x => x.GameProviderId)
                    .ToArray();
            }

            CachedPlayer? cachedPlayer = null;

            if (playerTokenData != null)
            {
                if (playerTokenData.PlatformType != query.PlatformType)
                {
                    throw new InvalidOperationException("request platform is not matched with player login platform.");
                }

                cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerTokenData.PlayerCode);
                if (cachedPlayer != null && cachedPlayer.DefaultWalletCurrencyCode.IgnoreCaseNotEquals(currencyCode))
                {
                    throw new CurrencyNotMatchingException(cachedPlayer.DefaultWalletCurrencyCode, currencyCode);
                }
            }

            var playerType = cachedPlayer?.Type ?? PlayerType.RealPlayer;
            var types = query.Types ?? Array.Empty<GameType>();
            var cultureCode = await _brandLanguageCodeMapper.ExternalToInternalAsync(query.LanguageCode, query.BrandId);

            var translatedGames = new List<GameInfo>();
            foreach (var gameProviderId in gameProviderIds)
            {
                var games = await GetGameModelsAsync(query.BrandId, gameProviderId, query.PlatformType, currencyCode, playerType);
                var store = await _gameQueries.GetGamesLocalizationByGameIdsAsync(games.Select(x => x.Id).ToArray());

                var translatedGame = games
                    .Where(g => types.Length == 0 || types.Contains(g.Type))
                    .Select(g =>
                    {
                        var key = g.Id.ToString();

                        var gameName = g.Name;
                        var gameDesc = g.Description;

                        if (store.TryGetValue(g.Id, out var value))
                        {
                            var localization = JsonDocument.Parse(value);

                            localization.RootElement.TryGetProperty(query.LanguageCode, out var langName);
                            if (langName.ValueKind != JsonValueKind.Undefined)
                            {
                                if (langName.TryGetProperty("name", out var localizationName)
                                    && !string.IsNullOrEmpty(localizationName.GetString()))
                                    gameName = localizationName.GetString();
                                if (langName.TryGetProperty("description", out var localizationDesc)
                                    && !string.IsNullOrEmpty(localizationDesc.GetString()))
                                    gameDesc = localizationDesc.GetString();
                            }
                        }
                        return g with
                        {
                            Name = gameName!,
                            Description = gameDesc!
                        };
                    }).ToArray();

                translatedGames.AddRange(translatedGame);
            }

            foreach (var item in translatedGames)
            {
                yield return item;

                if (cancellationToken.IsCancellationRequested)
                    yield break;
            }
        }

        private async Task<GameInfo[]> GetGameModelsAsync(
            Guid brandId,
            Guid gameProviderId,
            PlatformType platformType,
            string currencyCode,
            PlayerType playerType)
        {
            var statuses = _validStatus.GetStatus(playerType);
            var gameStatuses = _validGameStatus.GetStatus(playerType);

            if (currencyCode.IsNotNullOrEmpty())
            {
                var isCurrencySupported =
                    await _gameProviderOperations.CurrencySupportedAsync(gameProviderId, currencyCode);

                if (!isCurrencySupported)
                {
                    return Array.Empty<GameInfo>();
                }
            }

            var gamePlatformTypes = platformType.ToGamePlatformTypes();
            var brandGameProviderStatus =
                await _brandOperations.GetBrandGameProviderStatusAsync(brandId, gameProviderId) ?? Status.Active;
            var brandGames = await _brandOperations.GetBrandGamesAsync(brandId, gamePlatformTypes);
            var cachedGameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(gameProviderId);
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId);

            cachedGames = cachedGames
                .Where(g => g.Status != GameStatus.Inactive
                    && g.Status != GameStatus.Archived)
                .ToArray();

            return (from bg in brandGames.Where(g => g.GameProviderId == gameProviderId)
                    join g in cachedGames on bg.GameId equals g.Id
                    where g.AvailableToBrand
                        && (g.AvailableCurrencies.Length == 0
                            || g.AvailableCurrencies.Contains(currencyCode, StringComparer.OrdinalIgnoreCase))
                    select new GameInfo
                    {
                        Id = bg.GameId,
                        Code = g.Code,
                        Name = g.Name,
                        Description = g.Description,
                        GameProviderCode = cachedGameProvider.Code,
                        Type = g.Type,
                        IsActive = statuses.Contains(cachedGameProvider.Status)
                            && gameStatuses.Contains(g.Status)
                            && statuses.Contains(brandGameProviderStatus)
                            && gameStatuses.Contains(bg.BrandGameStatus),
                        Tag = new TagInfo()
                        {
                            New = bg.Tag.IsNew(),
                            Hot = bg.Tag.IsHot(),
                            Recommend = bg.Tag.IsRecommend()
                        },
                        DisplayOrder = bg.DisplayOrder
                    })
                .ToArray();
        }
    }
}