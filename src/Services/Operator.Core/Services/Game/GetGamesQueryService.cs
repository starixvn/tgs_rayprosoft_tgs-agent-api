using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operator.GameProviders;
using TGS.Services.Operator.Games;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class GetGamesQueryService : IQueryService<GetGamesQuery, GameInfo>
    {
        private readonly IBrandCurrencyCodeMapper _brandCurrencyCodeMapper;
        private readonly IBrandLanguageCodeMapper _brandLanguageCodeMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IPlayerOperations _playerOperations;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IValidStatus<Status> _validStatus;

        public GetGamesQueryService(
            IBrandCurrencyCodeMapper brandCurrencyCodeMapper,
            IBrandOperations brandOperations,
            IPlayerOperations playerOperations,
            IBrandLanguageCodeMapper brandLanguageCodeMapper,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus,
            IGameProviderRepository gameProviderRepository,
            IGameRepository gameRepository)
        {
            _brandCurrencyCodeMapper = brandCurrencyCodeMapper ??
                throw new ArgumentNullException(nameof(brandCurrencyCodeMapper));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandLanguageCodeMapper = brandLanguageCodeMapper ??
                throw new ArgumentNullException(nameof(brandLanguageCodeMapper));
            _validStatus = validStatus ?? throw new ArgumentNullException(nameof(validStatus));
            _validGameStatus = validGameStatus ?? throw new ArgumentNullException(nameof(validGameStatus));
            _gameProviderRepository =
                gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
            _gameRepository = gameRepository ?? throw new ArgumentNullException(nameof(gameRepository));
        }

        public async IAsyncEnumerable<GameInfo> QueryAsync(
            GetGamesQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var playerTokenData = query.PlayerTokenData;
            var currencyCode =
                await _brandCurrencyCodeMapper.ExternalToInternalAsync(query.CurrencyCode, query.BrandId);

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
            var cultureCode = await _brandLanguageCodeMapper.ExternalToInternalAsync(query.LanguageCode, query.BrandId);

            var gamePlatformTypes = query.PlatformType.ToGamePlatformTypes();
            var statuses = _validStatus.GetStatus(playerType);
            var gameStatuses = _validGameStatus.GetStatus(playerType);

            var subGameProviderCode = query.GameProviderCode;
            var brandGameProviders =
                await _brandOperations.GetBrandGameProvidersAsync(query.BrandId).ConfigureAwait(false);
            var brandGames = await _brandOperations.GetBrandGamesAsync(query.BrandId).ConfigureAwait(false);

            var subGameProviders = await _gameProviderRepository.QuerySubGameProvidersAsync(subGameProviderCode)
                .ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var gameProviderIds = (from bgp in brandGameProviders
                                   join sbp in subGameProviders on bgp.GameProviderId equals sbp.GameProviderId
                                   select sbp.GameProviderId).ToArray();

            var gameProviders =
                await _gameProviderRepository.QueryGameProvidersAsync(
                        gameProviderIds,
                        new[]
                        {
                            Status.Active,
                            Status.Maintenance
                        })
                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var availableGameProviderIds = gameProviders.Select(gp => gp.Id).ToArray();
            var gameProviderCurrencies = await _gameProviderRepository
                .QueryGameProviderCurrenciesAsync(availableGameProviderIds)
                .ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var queryGameProviderIds = (from bg in brandGames
                                        join c in gameProviderCurrencies on bg.GameProviderId equals c.GameProviderId
                                        where gamePlatformTypes.Contains(bg.PlatformType) &&
                                            (string.IsNullOrEmpty(currencyCode) ||
                                                c.CurrencyCode.IgnoreCaseEquals(currencyCode))
                                        select bg.GameProviderId).Distinct().ToArray();

            var games = await _gameRepository.QueryGamesAsync(
                    queryGameProviderIds,
                    new[]
                    {
                        GameStatus.Active,
                        GameStatus.Testing,
                        GameStatus.Maintenance
                    },
                    gamePlatformTypes)
                .ToArrayAsync(cancellationToken).ConfigureAwait(true);

            var infos = from g in games
                        join bg in brandGames on g.Id equals bg.GameId
                        let availableCurrencies = !string.IsNullOrEmpty(g.AvailableCurrencies)
                            ? g.AvailableCurrencies.SplitByComma()
                            : Array.Empty<string>()
                        let gp = gameProviders.First(gp => gp.Id == bg.GameProviderId)
                        let bgp = brandGameProviders.First(bgp => bgp.GameProviderId == bg.GameProviderId)
                        where g.AvailableToBrand &&
                            (availableCurrencies.Length == 0 ||
                                availableCurrencies.IgnoreCaseContains(currencyCode)) &&
                            (string.IsNullOrEmpty(subGameProviderCode) ||
                                g.SubGameProviderCode.IgnoreCaseEquals(subGameProviderCode))
                        let isActive = statuses.Contains(gp.Status) &&
                            statuses.Contains(bgp.BrandGameProviderStatus) &&
                            gameStatuses.Contains(bg.BrandGameStatus) &&
                            gameStatuses.Contains(g.Status)
                        select GetGameModels(g, bg, isActive, cultureCode);

            foreach (var item in infos.OrderBy(g => g.Id))
            {
                yield return item;

                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
            }
        }

        private static GameInfo GetGameModels(
            GameDataInfo game,
            CachedBrandGame brandGame,
            bool isActive,
            string cultureCode)
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
                DisplayOrder = brandGame.DisplayOrder
            };

            if (string.IsNullOrEmpty(game.Localization))
            {
                return info;
            }

            var localization = JsonDocument.Parse(game.Localization);
            if (!localization.RootElement.TryGetProperty(cultureCode, out var lang) ||
                lang.ValueKind == JsonValueKind.Undefined)
            {
                return info;
            }

            return info with
            {
                Name = GetLocalization(lang, "name", info.Name),
                Description = GetLocalization(lang, "description", info.Name)
            };
        }

        private static string GetLocalization(JsonElement element, string propertyName, string defaultValue)
        {
            if (!element.TryGetProperty(propertyName, out var value))
            {
                return defaultValue;
            }

            var val = value.GetString();
            return string.IsNullOrEmpty(val) ? defaultValue : val;
        }
    }
}