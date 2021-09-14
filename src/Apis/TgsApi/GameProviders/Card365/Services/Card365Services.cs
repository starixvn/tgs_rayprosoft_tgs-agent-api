using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Services
{
    public interface ICard365Services
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync();
    }

    public class Card365Services : ICard365Services
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;

        public Card365Services(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations)
        {
            _httpContext = httpContextAccessor?.HttpContext ??
                throw new ArgumentNullException(nameof(httpContextAccessor));

            _gameProviderOperations =
                gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }

        public async ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync()
        {
            var items = _httpContext?.Items ??
                throw new InvalidOperationException("Can not get http context items");

            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            var gameProviderConfigurationId = items.Get(Consts.ContextItems.GameProviderConfigurationId, Guid.Empty);
            var platformType = items.Get(Consts.ContextItems.PlatformType, PlatformType.Desktop);

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