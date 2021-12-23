using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings.JDB;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Services
{
    public interface IJDBServices
    {
        JDBAgent GetAgent(IEnumerable<JDBAgent> agents, string currencyCode);

        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(PlatformType platformType);
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

        JDBAgent IJDBServices.GetAgent(IEnumerable<JDBAgent> agents, string currencyCode)
            => agents?
                .FirstOrDefault(x => x.CurrencyCode.IgnoreCaseEquals(currencyCode));

        async ValueTask<GameProviderConfigurationData> IJDBServices.GetGameProviderConfigurationDataAsync(
                    PlatformType platformType)
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
    }
}
