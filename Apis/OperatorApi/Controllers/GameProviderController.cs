using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages.GameProviders;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using static TGS.Core.Models.Consts.LocalizationContext;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class GameProviderController : ControllerBase
    {
        private readonly IRedisCacheManager _cacheManager;
        private readonly IJsonSerializationProvider _json;

        private readonly ILicenseeCurrencyCodeMapper _licenseeCurrencyCodeMapper;
        private readonly ILicenseeLanguageCodeMapper _licenseeLanguageCodeMapper;
        private readonly ILicenseeOperations _licenseeOperations;
        private readonly ILocalizationOperations _localizationOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IValidStatus<Status> _validStatus;

        public GameProviderController(
            ILicenseeOperations licenseeOperations,
            IPlayerOperations playerOperations,
            ILocalizationOperations localizationOperations,
            ILicenseeLanguageCodeMapper licenseeLanguageCodeMapper,
            ILicenseeCurrencyCodeMapper licenseeCurrencyCodeMapper,
            IValidStatus<Status> validStatus,
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json)
        {
            _licenseeOperations = licenseeOperations;
            _playerOperations = playerOperations;
            _localizationOperations = localizationOperations;
            _licenseeLanguageCodeMapper = licenseeLanguageCodeMapper;
            _licenseeCurrencyCodeMapper = licenseeCurrencyCodeMapper;
            _validStatus = validStatus;
            _cacheManager = cacheManager;
            _json = json;
        }

        /// <summary>
        /// Get game provider list
        /// </summary>
        [HttpPost]
        [Route("list")]
        public async Task<GetGameProvidersResponse> GetGameProvidersAsync(GetGameProvidersRequest request)
        {
            var licenseeId = HttpContext.Items.GetLicenseeId();
            var currencyCode =
                await _licenseeCurrencyCodeMapper.ExternalToInternalAsync(request.CurrencyCode, licenseeId);

            var playerCode = request.PlayerCode;
            var cachedPlayer = playerCode.IsNotNullOrEmpty()
                ? await _playerOperations.GetCachedPlayerAsync(playerCode)
                : null;

            if (cachedPlayer != null && cachedPlayer.DefaultWalletCurrencyCode.IgnoreCaseNotEquals(currencyCode))
            {
                throw new CurrencyNotMatchingException(cachedPlayer.DefaultWalletCurrencyCode, currencyCode);
            }

            var platformType = request.PlatformType;
            var playerType = cachedPlayer?.Type ?? PlayerType.RealPlayer;
            var culture = await _licenseeLanguageCodeMapper.ExternalToInternalAsync(request.Language, licenseeId);

            var cacheType = RedisCacheType.LicenseeGameProviders;
            var field = $"{licenseeId}:{currencyCode}:{platformType}:{(int) playerType}:{culture}";
            var value = await _cacheManager.SafeGetHashSetAsync(cacheType, null, field);
            if (value.HasValue)
            {
                return new GetGameProvidersResponse
                {
                    GameProviders = _json.DeserializeFromString<GetGameProvidersResponse.GameProviderModel[]>(value)
                };
            }

            var providers = await GetGameProviderModels(licenseeId, currencyCode, platformType, playerType, culture);

            await _cacheManager.SafeCacheHashSetAsync(cacheType, null, new HashEntry[]
            {
                new (field, _json.SerializeToString(providers))
            }, TimeSpan.FromHours(12));

            return new GetGameProvidersResponse
            {
                GameProviders = providers
            };
        }

        private async Task<GetGameProvidersResponse.GameProviderModel[]> GetGameProviderModels(Guid licenseeId,
            string currencyCode, PlatformType platformType,
            PlayerType playerType, string culture)
        {
            var gameProviders = await _licenseeOperations.GetLicenseeGameProvidersAsync(licenseeId, currencyCode,
                platformType);

            var statuses = _validStatus.GetStatus(playerType);

            return await Task.WhenAll(gameProviders
                .Where(gp => statuses.Contains(gp.Status))
                .Select(async gp =>
                {
                    var key = gp.Id.ToString();
                    var store = await _localizationOperations.GetLocalizationByGameProviderIdAsync(gp.Id, culture);
                    return new GetGameProvidersResponse.GameProviderModel
                    {
                        Id = gp.Id,
                        Code = gp.Code,
                        Name = _localizationOperations.Parse(store, key, GameProviderName, gp.Name),
                        Description =
                            _localizationOperations.Parse(store, key, GameProviderDescription, gp.Description),
                        Status = gp.Status
                    };
                }));
        }
    }
}