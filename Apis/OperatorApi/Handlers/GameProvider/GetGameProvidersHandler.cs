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

            const RedisCacheType cacheType = RedisCacheType.BrandGameProvidersWithConditions;
            var cacheKey = brandId;
            var field = $"{currencyCode}:{platformType}:{(int) playerType}";
            var value = await _cacheManager.SafeGetHashSetAsync(cacheType, cacheKey, field);

            GameProviderModel[] providers;

            if (value.HasValue)
            {
                providers = _json.DeserializeFromString<GameProviderModel[]>(value);
            }
            else
            {
                providers = await GetGameProviderModels(brandId, currencyCode, platformType, playerType);

                await _cacheManager.SafeCacheHashSetAsync(cacheType, cacheKey, new HashEntry[]
                {
                    new(field, _json.SerializeToString(providers))
                }, TimeSpan.FromHours(12));
            }

            var translatedProviders = await providers.Select(async gpm =>
            {
                var key = $"{gpm.Id}";
                var store = await _localizationOperations.GetLocalizationByGameProviderIdAsync(gpm.Id, culture);

                return gpm with
                {
                    Name = _localizationOperations.Parse(store, key, GameProviderDescription, gpm.Name),
                    Description = _localizationOperations.Parse(store, key, GameProviderDescription, gpm.Description)
                };
            }).WhenAll();

            return new()
            {
                GameProviders = translatedProviders
            };
        }

        private async Task<GameProviderModel[]> GetGameProviderModels(Guid brandId,
            string currencyCode, PlatformType platformType,
            PlayerType playerType)
        {
            var gameProviders = await _brandOperations.GetBrandGameProvidersAsync(brandId);
            var statuses = _validStatus.GetStatus(playerType);
            
            var providers = await gameProviders
                .Where(bgp => statuses.Contains(bgp.BrandGameProviderStatus))
                .Select(async bgp =>
                {
                    var gameProviderId = bgp.GameProviderId;
                    var gameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(bgp.GameProviderId);
                    var currencySupported =
                        await _gameProviderOperations.CurrencySupportedAsync(gameProviderId, currencyCode);

                    if (!currencySupported)
                    {
                        return null;
                    }

                    var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, platformType);

                    if (games.Length == 0)
                    {
                        return null;
                    }

                    return new GameProviderModel
                    {
                        Id = gameProviderId,
                        Code = gameProvider.Code,
                        Name = gameProvider.Name,
                        Description = gameProvider.Description,
                        Status = gameProvider.Status,
                        GameTypes = games.Select(g => g.Type).OrderBy(t => (int) t)
                            .Distinct().ToArray()
                    };
                }).WhenAll();

            return providers.Where(gpm => gpm != null).ToArray();
        }
    }
}