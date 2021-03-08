using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Services
{
    public interface IHabaneroServices
    {
        Task<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync();
    }

    public sealed class HabaneroServices : IHabaneroServices
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;

        public HabaneroServices(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
        }

        async Task<GameProviderConfigurationData> IHabaneroServices.GetGameProviderConfigurationDataAsync()
        {
            var items = _httpContext?.Items;
            if (items == null)
            {
                throw new InvalidOperationException("Can not get http context items");
            }

            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId);

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