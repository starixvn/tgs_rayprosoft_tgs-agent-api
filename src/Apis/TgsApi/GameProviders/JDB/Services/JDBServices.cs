using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.JDB;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Services
{
    public interface IJDBServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(PlatformType platformType);
        JDBBrandMerchant GetBrandMerchant(JDBConfiguration configuration, string brandCode);
        JDBAgentMapper GetAgentMapper(JDBBrandMerchant brandMerchant, string currencyCode);
    }

    public sealed class JDBServices : IJDBServices
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;

        public JDBServices(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
        }

        async ValueTask<GameProviderConfigurationData> IJDBServices.GetGameProviderConfigurationDataAsync(PlatformType platformType)
        {
            var items = _httpContext?.Items ??
                throw new InvalidOperationException("Can not get http context items");

            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);

            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId)
                .ConfigureAwait(false);

            return new()
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                AutoCloseRound = autoCloseRound,
                PlatformType = platformType
            };
        }

        JDBBrandMerchant IJDBServices.GetBrandMerchant(JDBConfiguration configuration, string brandCode)
        {
            var brandMerchant = configuration?.BrandMerchants?
                .Where(x => x.BrandCode.IgnoreCaseEquals(brandCode) && x.Status)
                .FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration?.BrandMerchants?
                    .Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status)
                    .FirstOrDefault();
            }

            return brandMerchant;
        }

        JDBAgentMapper IJDBServices.GetAgentMapper(JDBBrandMerchant brandMerchant, string currencyCode)
        {
            return brandMerchant?.AgentMappers?
                .Where(x => x.CurrencyCode.IgnoreCaseEquals(currencyCode) && x.Status)
                .FirstOrDefault();
        }
    }
}