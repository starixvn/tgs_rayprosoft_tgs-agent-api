// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.IMESports.Incoming;
using static TGS.Core.Models.Enums.GameActionType;

namespace TGS.Apis.TgsApi.GameProviders.IMESports.Services
{
    public interface IIMESportsServices
    {
        string GetRedisCacheKey(string playerCode, string externalGameId);
        
        ValueTask<string> GetPlayerCodeAsync(string imPlayerCode, IMESportsConfiguration config = null);

        ValueTask<IMESportsBrandMerchant> GetBrandMerchantAsync(string playerCode, IMESportsConfiguration config = null);
        
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId,
            Guid gameProviderConfigurationId, PlatformType platformType);
        
        ValueTask<IDictionary<string, IEnumerable<GameActionExternalIds>>> GetValidGameActionExternalIdsAsync(Guid gameProviderId,
            TransactionRequest[] transactionRequests);
    }

    public sealed class IMESportsServices : IIMESportsServices
    {
        private readonly IIMESportsServices _inferface;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public IMESportsServices(
            IGameActionQueries gameActionQueries,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            ISettingOperations settingOperations)
        {
            _inferface = this;
            _gameActionQueries = gameActionQueries;
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _settingOperations = settingOperations;
        }

        string IIMESportsServices.GetRedisCacheKey(string playerCode, string externalGameId)
        {
            return $"{playerCode}_{externalGameId}";
        }
        
        async ValueTask<string> IIMESportsServices.GetPlayerCodeAsync(string imPlayerCode, IMESportsConfiguration config)
        {
            var strArrays = imPlayerCode.SplitByUnderscore();
            
            var playerCode = strArrays[1];
            var merchant = await _inferface.GetBrandMerchantAsync(playerCode, config);
            
            if (strArrays[0].IgnoreCaseNotEquals(merchant.Prefix))
            {
                throw new ArgumentException($"Invalid prefix {strArrays[0]}, should be {merchant.Prefix}");
            }

            return playerCode;
        }

        async ValueTask<IMESportsBrandMerchant> IIMESportsServices.GetBrandMerchantAsync(string playerCode, IMESportsConfiguration config)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode);
            config ??=
                await _settingOperations.GetValueFromJsonByKeyAsync<IMESportsConfiguration>(SettingConsts
                    .GameProviderConfigurations.IMESports);
            var merchant = config.BrandMerchants.FirstOrDefault(b => b.BrandCode.IgnoreCaseEquals(player.BrandCode));
            return merchant ?? config.BrandMerchants.FirstOrDefault(b => b.BrandCode.IgnoreCaseEquals("Default"));
        }
        
        async ValueTask<GameProviderConfigurationData> IIMESportsServices.GetGameProviderConfigurationDataAsync(
            Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType)
        {
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
        
        async ValueTask<IDictionary<string, IEnumerable<GameActionExternalIds>>> IIMESportsServices.GetValidGameActionExternalIdsAsync(
            Guid gameProviderId, TransactionRequest[] transactionRequests)

        {
            if (!transactionRequests.Any())
            {
                throw new InvalidOperationException("TransactionRequest[] cannot be empty");
            }
            
            var actionTypes = new[] {PlaceBet, WinBet, LoseBet, CancelTransaction};
            var externalGameId = transactionRequests[0].ExternalGameId;
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId);
            var config =
                await _settingOperations.GetValueFromJsonByKeyAsync<IMESportsConfiguration>(SettingConsts
                    .GameProviderConfigurations.IMESports);
            
            var results = new Dictionary<string, IEnumerable<GameActionExternalIds>>();
            foreach (var tran in transactionRequests)
            {
                var playerCode = await _inferface.GetPlayerCodeAsync(tran.PlayerCode, config);
                var externalIds = await _gameActionQueries
                    .GetGameActionExternalIdsAsync(gameProviderId,
                        playerCode, tran.ExternalRoundId, cachedGames[0].Id, actionTypes).ConfigureAwait(false);
                results.Add(tran.ExternalRoundId, GetValidGameActions(externalIds));
            }
            
            return results;
        }

        private IEnumerable<GameActionExternalIds> GetValidGameActions(GameActionExternalIds[] externalIds)
        {
            var cancelledIds = externalIds.Where(e => e.GameActionType == CancelTransaction)
                .Select(e => e.ExternalReferencedTransactionId);
            var uncancelledIds = externalIds.Where(e => e.GameActionType == CancelTransaction && 
                                                        cancelledIds.IgnoreCaseContains(e.ExternalTransactionId))
                .Select(e => e.ExternalReferencedTransactionId);
            return externalIds.Where(e => cancelledIds.IgnoreCaseNotContains(e.ExternalTransactionId) ||
                                          uncancelledIds.IgnoreCaseContains(e.ExternalTransactionId));
        }
    }
}