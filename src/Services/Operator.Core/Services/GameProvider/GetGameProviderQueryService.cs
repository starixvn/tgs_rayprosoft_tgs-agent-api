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
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.GameProvider
{
    internal class GetGameProviderQueryService : IQueryService<GetGameProviderQuery, GameProviderModel>
    {
        private readonly IBrandCurrencyCodeMapper _brandCurrencyCodeMapper;
        private readonly IPlayerOperations _playerOperations;
        private readonly IBrandLanguageCodeMapper _brandLanguageCodeMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IValidStatus<Status> _validStatus;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IGameProviderOperations _gameProviderOperations;

        public GetGameProviderQueryService(
            IBrandCurrencyCodeMapper brandCurrencyCodeMapper,
            IPlayerOperations playerOperations,
            IBrandLanguageCodeMapper brandLanguageCodeMapper,
            IBrandOperations brandOperations,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus,
            IGameProviderOperations gameProviderOperations)
        {
            _brandCurrencyCodeMapper = brandCurrencyCodeMapper ?? throw new ArgumentNullException(nameof(brandCurrencyCodeMapper));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandLanguageCodeMapper = brandLanguageCodeMapper ?? throw new ArgumentNullException(nameof(brandLanguageCodeMapper));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _validStatus = validStatus ?? throw new ArgumentNullException(nameof(validStatus));
            _validGameStatus = validGameStatus ?? throw new ArgumentNullException(nameof(validGameStatus));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
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
            var culture = await _brandLanguageCodeMapper.ExternalToInternalAsync(query.LanguageCode, brandId);
            var providers = await GetGameProviderModelsAsync(brandId, currencyCode, platformType, playerType);
            var store = await _gameProviderOperations.GetGameProvidersLocalizationByGameProviderIdsAsync(providers.Select(x => x.Id).ToArray());

            foreach (var gpm in providers)
            {
                var key = $"{gpm.Id}";

                var gameProviderName = gpm.Name;
                var gameProviderDesc = gpm.Description;

                var gameProviderLocalization = store.FirstOrDefault(x => x.Id == gpm.Id);
                if (gameProviderLocalization != null)
                {
                    var localization = JsonDocument.Parse(gameProviderLocalization.Localization);

                    localization.RootElement.TryGetProperty(query.LanguageCode, out var langName);
                    if (langName.ValueKind != JsonValueKind.Undefined)
                    {
                        if (langName.TryGetProperty("name", out var localizationName)
                                && !string.IsNullOrEmpty(localizationName.GetString()))
                            gameProviderName = localizationName.GetString();
                        if (langName.TryGetProperty("description", out var localizationDesc)
                                && !string.IsNullOrEmpty(localizationDesc.GetString()))
                            gameProviderDesc = localizationDesc.GetString();
                    }
                }

                var model = gpm with
                {
                    Name = gameProviderName!,
                    Description = gameProviderDesc!
                };

                yield return model;

                if (cancellationToken.IsCancellationRequested)
                    yield break;
            }
        }

        private async Task<GameProviderModel[]> GetGameProviderModelsAsync(
            Guid brandId,
            string currencyCode,
            PlatformType platformType,
            PlayerType playerType)
        {
            var gameProviders = await _brandOperations.GetBrandGameProvidersAsync(brandId);
            var statuses = _validStatus.GetStatus(playerType);
            var gameStatuses = _validGameStatus.GetStatus(playerType);

            var providers = new List<GameProviderModel>();

            foreach (var bgp in gameProviders.Where(bgp => statuses.Contains(bgp.BrandGameProviderStatus)))
            {
                var gameProviderId = bgp.GameProviderId;
                var gameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(bgp.GameProviderId);
                var currencySupported = await _gameProviderOperations.CurrencySupportedAsync(gameProviderId, currencyCode);

                if (!currencySupported)
                {
                    continue;
                }

                var gamePlatformTypes = platformType.ToGamePlatformTypes();
                var allGames = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, gamePlatformTypes);
                var games = allGames.Where(g => gameStatuses.Contains(g.Status)).ToArray();

                if (games.Length == 0)
                {
                    continue;
                }

                var model = new GameProviderModel
                {
                    Id = gameProviderId,
                    Code = gameProvider.Code,
                    Name = gameProvider.Name,
                    Description = gameProvider.Description,
                    Status = gameProvider.Status,
                    IsTransferWallet = gameProvider.IsTransferWallet,
                    GameTypes = games.Select(g => g.Type)
                        .OrderBy(t => (int)t)
                        .Distinct()
                        .ToArray()
                };

                providers.Add(model);
            }

            return providers.Where(gpm => gpm != null).ToArray();
        }
    }
}