using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using static TGS.Core.Models.Consts.LocalizationContext;

namespace TGS.Apis.OperatorApi.Handlers.GameProvider
{
    public sealed class GetGameProvidersHandler : IRequestHandler<GetGameProvidersRequest, GetGameProvidersResponse>
    {
        private readonly IBrandCurrencyCodeMapper _brandCurrencyCodeMapper;
        private readonly IBrandLanguageCodeMapper _brandLanguageCodeMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IRedisCacheManager _cacheManager;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;
        private readonly IJsonSerializationProvider _json;
        private readonly ILocalizationOperations _localizationOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IValidStatus<Status> _validStatus;

        public GetGameProvidersHandler(
            IHttpContextAccessor httpContextAccessor,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            ILocalizationOperations localizationOperations,
            IBrandLanguageCodeMapper brandLanguageCodeMapper,
            IBrandCurrencyCodeMapper brandCurrencyCodeMapper,
            IValidStatus<Status> validStatus,
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _localizationOperations = localizationOperations;
            _brandLanguageCodeMapper = brandLanguageCodeMapper;
            _brandCurrencyCodeMapper = brandCurrencyCodeMapper;
            _validStatus = validStatus;
            _cacheManager = cacheManager;
            _json = json;
        }

        async Task<GetGameProvidersResponse> IRequestHandler<GetGameProvidersRequest, GetGameProvidersResponse>.Handle(
            GetGameProvidersRequest request, CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();
            var currencyCode =
                await _brandCurrencyCodeMapper.ExternalToInternalAsync(request.CurrencyCode, brandId);

            var playerTokenData = _httpContext.Items.GetPlayerTokenData();
            var platformType = request.PlatformType;

            CachedPlayer cachedPlayer = null;

            if (playerTokenData != null)
            {
                if (playerTokenData.PlatformType != platformType)
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
            var culture = await _brandLanguageCodeMapper.ExternalToInternalAsync(request.LanguageCode, brandId);

            var cacheType = RedisCacheType.BrandGameProvidersWithConditions;
            var cacheKey = $"{brandId}";
            var field = $"{currencyCode}:{platformType}:{(int) playerType}:{culture}";

            var value = await _cacheManager.SafeGetHashSetAsync(cacheType, cacheKey, field);
            if (value.HasValue)
            {
                return new()
                {
                    GameProviders = _json.DeserializeFromString<GetGameProvidersResponse.GameProviderModel[]>(value)
                };
            }

            var providers = await GetGameProviderModels(brandId, currencyCode, platformType, playerType, culture);

            await _cacheManager.SafeCacheHashSetAsync(cacheType, cacheKey, new HashEntry[]
            {
                new(field, _json.SerializeToString(providers))
            }, TimeSpan.FromHours(12));

            return new()
            {
                GameProviders = providers
            };
        }

        private async Task<GetGameProvidersResponse.GameProviderModel[]> GetGameProviderModels(Guid brandId,
            string currencyCode, PlatformType platformType,
            PlayerType playerType, string culture)
        {
            var gameProviders = await _brandOperations.GetBrandGameProvidersAsync(brandId);
            var statuses = _validStatus.GetStatus(playerType);

            var items = new List<GetGameProvidersResponse.GameProviderModel>();

            foreach (var bgp in gameProviders
                .Where(bgp => statuses.Contains(bgp.BrandGameProviderStatus)))
            {
                var gameProviderId = bgp.GameProviderId;
                var gameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(bgp.GameProviderId);
                var currencySupported =
                    await _gameProviderOperations.CurrencySupportedAsync(gameProviderId, currencyCode);

                if (!currencySupported)
                {
                    continue;
                }

                var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, platformType);

                if (games.Length == 0)
                {
                    continue;
                }

                var key = $"{gameProviderId}";
                var store = await _localizationOperations.GetLocalizationByGameProviderIdAsync(gameProviderId, culture);

                var model = new GetGameProvidersResponse.GameProviderModel
                {
                    Id = gameProviderId,
                    Code = gameProvider.Code,
                    Name = _localizationOperations.Parse(store, key, GameProviderName, gameProvider.Name),
                    Description =
                        _localizationOperations.Parse(store, key, GameProviderDescription, gameProvider.Description),
                    Status = gameProvider.Status
                };

                items.Add(model);
            }

            return items.ToArray();
        }
    }
}