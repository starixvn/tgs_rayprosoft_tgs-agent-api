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
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.GameProvider
{
    internal class GetGameProviderQueryService : IQueryService<GetGameProviderQuery, GameProviderModel>
    {
        private readonly IBrandCurrencyCodeMapper _brandCurrencyCodeMapper;
        private readonly IBrandLanguageCodeMapper _brandLanguageCodeMapper;

        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IPlayerOperations _playerOperations;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IValidStatus<Status> _validStatus;

        public GetGameProviderQueryService(
            IBrandCurrencyCodeMapper brandCurrencyCodeMapper,
            IPlayerOperations playerOperations,
            IBrandLanguageCodeMapper brandLanguageCodeMapper,
            IBrandOperations brandOperations,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus,
            IGameProviderRepository gameProviderRepository)
        {
            _brandCurrencyCodeMapper = brandCurrencyCodeMapper ??
                throw new ArgumentNullException(nameof(brandCurrencyCodeMapper));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandLanguageCodeMapper = brandLanguageCodeMapper ??
                throw new ArgumentNullException(nameof(brandLanguageCodeMapper));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _validStatus = validStatus ?? throw new ArgumentNullException(nameof(validStatus));
            _validGameStatus = validGameStatus ?? throw new ArgumentNullException(nameof(validGameStatus));

            _gameProviderRepository =
                gameProviderRepository ?? throw new ArgumentNullException(nameof(gameProviderRepository));
        }

        public async IAsyncEnumerable<GameProviderModel> QueryAsync(
            GetGameProviderQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var brandId = query.BrandId;
            var currencyCode = await _brandCurrencyCodeMapper.ExternalToInternalAsync(query.CurrencyCode, brandId);

            var playerTokenData = query.PlayerTokenData;
            var platformType = query.PlatformType;

            CachedPlayer? cachedPlayer = null;

            if (playerTokenData != null)
            {
                if (playerTokenData.PlatformType != platformType)
                {
                    throw new InvalidOperationException("request platform is not matched with player login platform.");
                }

                cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerTokenData.PlayerCode);
                if (cachedPlayer != null
                    && cachedPlayer.DefaultWalletCurrencyCode.IgnoreCaseNotEquals(currencyCode))
                {
                    throw new CurrencyNotMatchingException(cachedPlayer.DefaultWalletCurrencyCode, currencyCode);
                }
            }

            var playerType = cachedPlayer?.Type ?? PlayerType.RealPlayer;
            var culture = await _brandLanguageCodeMapper.ExternalToInternalAsync(query.LanguageCode, brandId)
                .ConfigureAwait(false);

            var brandGameProviders = await _brandOperations.GetBrandGameProvidersAsync(brandId)
                .ConfigureAwait(false);

            var statuses = _validStatus.GetStatus(playerType);
            var gameStatuses = _validGameStatus.GetStatus(playerType);

            var gamePlatformTypes = platformType.ToGamePlatformTypes();
            var gameProviderIds = brandGameProviders.Where(bgp => statuses.Contains(bgp.BrandGameProviderStatus))
                .Select(bgp => bgp.GameProviderId).Distinct().ToArray();

            var gameProviders =
                await _gameProviderRepository.QueryGameProvidersAsync(gameProviderIds, gamePlatformTypes)
                    .ToArrayAsync(cancellationToken).ConfigureAwait(false);

            var subGameProviders = await _gameProviderRepository.QuerySubGameProvidersAsync(gameProviderIds)
                .ToArrayAsync(cancellationToken).ConfigureAwait(false);

            foreach (var gp in gameProviders.Where(gp => statuses.Contains(gp.Status)))
            {
                if (gp.Currencies.IgnoreCaseNotContains(currencyCode))
                {
                    continue;
                }

                foreach (var sgp in subGameProviders.Where(sgp => sgp.GameProviderId == gp.Id))
                {
                    var games = gp.Games.Where(g
                            => gameStatuses.Contains(g.Status) && g.SubGameProviderCode.IgnoreCaseEquals(sgp.Code))
                        .ToArray();

                    if (games.Length == 0)
                    {
                        continue;
                    }

                    var gameTypes = games.Select(g => g.Type)
                        .OrderBy(t => (int)t)
                        .Distinct()
                        .ToArray();

                    if (string.IsNullOrEmpty(sgp.Localization))
                    {
                        yield return new GameProviderModel
                        {
                            Code = sgp.Code,
                            Name = sgp.Name,
                            Description = sgp.Description,
                            Status = gp.Status,
                            IsTransferWallet = gp.IsTransferWallet,
                            GameTypes = gameTypes
                        };

                        continue;
                    }

                    var localization = JsonDocument.Parse(sgp.Localization);
                    if (!localization.RootElement.TryGetProperty(culture, out var lang) ||
                        lang.ValueKind == JsonValueKind.Undefined)
                    {
                        yield return new GameProviderModel
                        {
                            Code = sgp.Code,
                            Name = sgp.Name,
                            Description = sgp.Description,
                            Status = gp.Status,
                            IsTransferWallet = gp.IsTransferWallet,
                            GameTypes = gameTypes
                        };

                        continue;
                    }

                    yield return new GameProviderModel
                    {
                        Code = sgp.Code,
                        Name = GetLocalization(lang, "name", sgp.Name),
                        Description = GetLocalization(lang, "description", sgp.Description),
                        Status = gp.Status,
                        IsTransferWallet = gp.IsTransferWallet,
                        GameTypes = gameTypes
                    };
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }
            }
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