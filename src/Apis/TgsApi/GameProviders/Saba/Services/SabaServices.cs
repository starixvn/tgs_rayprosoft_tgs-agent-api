using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Extensions;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Services
{
    public interface ISabaServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync();

        ValueTask<decimal> GetLockAmountAsync(string playerCode, Guid gameProviderId,
            string externalRoundId);

        ValueTask<string> GetExternalRoundIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);

        ValueTask<GameActionExternalIds[]> GetGameActionExternalIdsByRoundIdAsync(Guid gameProviderId,
            string playerCode, string externalRoundId, int gameId);

        bool IsTestEnvironment(string[] acceptEnvironments);

        string GetPlayerCode(string userId, bool isTestEnv);
    }

    public sealed class SabaServices : ISabaServices
    {
        private readonly IWebHostEnvironment _env;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;

        public SabaServices(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IWebHostEnvironment env)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
            _env = env;
        }

        async ValueTask<GameProviderConfigurationData> ISabaServices.GetGameProviderConfigurationDataAsync()
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

        async ValueTask<decimal> ISabaServices.GetLockAmountAsync(string playerCode, Guid gameProviderId,
            string externalRoundId)
        {
            var game = (await _gameProviderOperations
                .GetCachedGamesAsync(gameProviderId, SabaConsts.ExternalGameId)
                .ConfigureAwait(false)).FirstOrDefault();

            var gameActionExternalId =
                (await _gameActionQueries.GetGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId
                    , game!.Id, new[] {GameActionType.LockAmount}).ConfigureAwait(false))
                .FirstOrDefault();

            if (gameActionExternalId == null)
            {
                throw new InvalidGameActionException($"No lock transaction is found in round '{externalRoundId}'",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            return gameActionExternalId.Amount;
        }

        async ValueTask<string> ISabaServices.GetExternalRoundIdAsync(Guid gameProviderId, string playerCode,
            string externalTransactionId)
        {
            var gameActionExternalId =
                await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode,
                    externalTransactionId).ConfigureAwait(false);

            if (gameActionExternalId == null)
            {
                throw new InvalidGameActionException($"No transaction of id '{externalTransactionId}' is found",
                    InvalidGameActionErrorCode.TransactionNotFound);
            }

            return gameActionExternalId.ExternalRoundId;
        }

        async ValueTask<GameActionExternalIds[]> ISabaServices.GetGameActionExternalIdsByRoundIdAsync(
            Guid gameProviderId, string playerCode, string externalRoundId, int gameId)
        {
            var gameActions = await _gameActionQueries
                .GetValidGameActionExternalIdsAsync(gameProviderId, playerCode, externalRoundId, gameId, false)
                .ConfigureAwait(false);

            return gameActions
                .Where(t => t.GameActionType.IsNotAllOf(GameActionType.LockAmount, GameActionType.UnlockAmount))
                .ToArray();
        }

        bool ISabaServices.IsTestEnvironment(string[] acceptEnvironments) =>
            acceptEnvironments.IgnoreCaseContains(_env.EnvironmentName);

        string ISabaServices.GetPlayerCode(string userId, bool isTestEnv) =>
            !isTestEnv ? userId : userId?.Replace("_test", string.Empty, StringComparison.OrdinalIgnoreCase);
    }
}