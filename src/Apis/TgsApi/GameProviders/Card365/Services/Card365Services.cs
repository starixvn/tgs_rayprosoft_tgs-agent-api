using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Services
{
    public interface ICard365Services
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync();

        ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsAsync(Guid gameProviderId, string playerCode,
            string externalRoundId);
    }

    public class Card365Services : ICard365Services
    {
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;

        public Card365Services(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries)
        {
            _httpContext = httpContextAccessor?.HttpContext ??
                throw new ArgumentNullException(nameof(httpContextAccessor));

            _gameProviderOperations =
                gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
        }

        public ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsAsync(Guid gameProviderId, string playerCode,
            string externalRoundId) => _gameActionQueries.GetGameActionExternalIdsByExternalGameActionGroupIdAsync(
            gameProviderId,
            playerCode, externalRoundId);

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