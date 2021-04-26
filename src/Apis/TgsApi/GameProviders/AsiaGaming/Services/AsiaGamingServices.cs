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

namespace TGS.Apis.TgsApi.GameProviders.AsiaGaming.Services
{
    public interface IAsiaGamingServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync();

        ValueTask<GameActionExternalIds> GetLockGameActionExternalIdsAsync(string playerCode, string transactionId);
    }

    public sealed class AsiaGamingServices : IAsiaGamingServices
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly HttpContext _httpContext;

        public AsiaGamingServices(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
        }

        async ValueTask<GameProviderConfigurationData> IAsiaGamingServices.GetGameProviderConfigurationDataAsync()
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

        async ValueTask<GameActionExternalIds> IAsiaGamingServices.GetLockGameActionExternalIdsAsync(string playerCode, string transactionId)
        {
            var items = _httpContext?.Items ?? throw new InvalidOperationException("Can not get http context items");
            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            
            var externalIds = await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode,
                transactionId).ConfigureAwait(false);

            if (externalIds.GameActionType != GameActionType.LockAmount)
            {
                throw new ArgumentException($"Wrong transaction id {transactionId}, game action type can't be {externalIds.GameActionType}");
            }

            return externalIds;
        }
    }
}