using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages.GameProviders;
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

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class GameProviderController : ControllerBase
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly IJsonSerializationProvider _json;
        private readonly IBrandCurrencyCodeMapper _brandCurrencyCodeMapper;
        private readonly IBrandLanguageCodeMapper _brandLanguageCodeMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ILocalizationOperations _localizationOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IValidStatus<Status> _validStatus;

        public GameProviderController(
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

        /// <summary>
        /// Get game provider list
        /// </summary>
        [HttpPost]
        [Route("list")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public async Task<GetGameProvidersResponse> GetGameProvidersAsync(GetGameProvidersRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var currencyCode =
                await _brandCurrencyCodeMapper.ExternalToInternalAsync(request.CurrencyCode, brandId);

            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
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
            var culture = await _brandLanguageCodeMapper.ExternalToInternalAsync(request.Language, brandId);

            var cacheType = RedisCacheType.BrandGameProvidersWithConditions;
            var cacheKey = $"{brandId}";
            var field = $"{currencyCode}:{platformType}:{(int) playerType}:{culture}";
           
            var value = await _cacheManager.SafeGetHashSetAsync(cacheType, cacheKey, field);
            if (value.HasValue)
            {
                return new GetGameProvidersResponse
                {
                    GameProviders = _json.DeserializeFromString<GetGameProvidersResponse.GameProviderModel[]>(value)
                };
            }

            var providers = await GetGameProviderModels(brandId, currencyCode, platformType, playerType, culture);

            await _cacheManager.SafeCacheHashSetAsync(cacheType, cacheKey, new HashEntry[]
            {
                new (field, _json.SerializeToString(providers))
            }, TimeSpan.FromHours(12));

            return new GetGameProvidersResponse
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