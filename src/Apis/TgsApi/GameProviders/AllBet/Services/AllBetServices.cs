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

namespace TGS.Apis.TgsApi.GameProviders.AllBet.Services
{
    public interface IAllBetServices
    {
        ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalGameActionGroupId);

        string GetPlayerCode(string allBetPlayerId, string suffix);

        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync();
    }

    public class AllBetServices : IAllBetServices
    {
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;

        public AllBetServices(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
        }

        async ValueTask<GameProviderConfigurationData> IAllBetServices.GetGameProviderConfigurationDataAsync()
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

        ValueTask<GameActionExternalIds[]> IAllBetServices.GetGameActionExternalIdsAsync(Guid gameProviderId,
            string playerCode, string externalGameActionGroupId)
            => _gameActionQueries.GetGameActionExternalIdsByExternalGameActionGroupIdAsync(
                gameProviderId, playerCode, externalGameActionGroupId);

        string IAllBetServices.GetPlayerCode(string allBetPlayerId, string suffix)
            => allBetPlayerId.Replace(suffix, string.Empty);
    }
}