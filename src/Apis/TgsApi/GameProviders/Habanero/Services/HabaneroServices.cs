using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.Habanero.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Habanero.Services
{
    public interface IHabaneroServices
    {
        Task<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync();
        Task<bool> GameActionExistsAsync(QueryDetail detail);
    }

    public sealed class HabaneroServices : IHabaneroServices
    {
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;

        public HabaneroServices(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
        }

        async Task<GameProviderConfigurationData> IHabaneroServices.GetGameProviderConfigurationDataAsync()
        {
            var items = _httpContext?.Items ??
                throw new InvalidOperationException("Can not get http context items");

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

        Task<bool> IHabaneroServices.GameActionExistsAsync(QueryDetail detail)
        {
            var items = _httpContext?.Items ?? throw new InvalidOperationException("Can not get http context items");

            var gameProviderId = items.Get(Consts.ContextItems.GameProviderId, Guid.Empty);
            return _gameActionQueries.ReferencedGameActionExistsAsync(gameProviderId, detail.PlayerCode,
                detail.RoundId, detail.TransactionId);
        }
    }
}