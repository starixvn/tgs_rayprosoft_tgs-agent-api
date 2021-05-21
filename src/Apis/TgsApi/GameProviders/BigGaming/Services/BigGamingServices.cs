using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.BigGaming;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.Models.BigGaming.Enums;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public interface IBigGamingServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType);
        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);
        ValueTask<decimal> GetPlayerBalanceAsync(string playerCode);
        ValueTask<BigGamingBrandMerchant> GetBrandMerchant(string currencyCode);
        ValueTask<Guid> GetGameProviderId();
        ValueTask<BigGamingOrderStatus> GetOrderStatusAsync(Guid gameProviderId, string playerCode, string orderId);
    }
    internal sealed class BigGamingServices : IBigGamingServices
    {
        private readonly ISettingOperations _settingOperations;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;

        public BigGamingServices(
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IPlayerOperations playerOperations)
        {
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
            _playerOperations = playerOperations;
        }


        async ValueTask<GameActionExternalIds> IBigGamingServices.GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
            => await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId).ConfigureAwait(false);

        async ValueTask<decimal> IBigGamingServices.GetPlayerBalanceAsync(string playerCode)
        {
            var walletId = await _playerOperations.GetDefaultWalletIdByPlayerCodeAsync(playerCode).ConfigureAwait(false);

            return await _playerOperations.GetPlayerBalanceAsync(walletId, false).ConfigureAwait(false);
        }

        async ValueTask<GameProviderConfigurationData> IBigGamingServices.GetGameProviderConfigurationDataAsync(
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

        async ValueTask<BigGamingOrderStatus> IBigGamingServices.GetOrderStatusAsync(Guid gameProviderId, string playerCode, string orderId)
        {
            var gameActionExternal = await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, orderId).ConfigureAwait(false);
            if (gameActionExternal == null)
            {
                return BigGamingOrderStatus.BetSlipDoesNotExist;
            }
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, gameActionExternal.ExternalGameId).ConfigureAwait(false);

            var gameActionData = await _gameActionQueries.GetGameActionsByExternalRoundIdAsync(gameProviderId, playerCode, gameActionExternal.ExternalRoundId, cachedGames[0].Id).ConfigureAwait(false);

            var gameActionRecordLast = (from gameActionRecord in gameActionData.GameActions
                                        where gameActionRecord.ExternalBetId == orderId
                                        orderby gameActionRecord.RecordedOn descending
                                        select gameActionRecord).First();

            return GetBGOrderStatus(gameActionRecordLast.Type);
        }

        async ValueTask<BigGamingBrandMerchant> IBigGamingServices.GetBrandMerchant(string currencyCode)
        {
            var configuration =
              await _settingOperations.GetValueFromJsonByKeyAsync<BigGamingConfiguration>(SettingConsts
                  .GameProviderConfigurations.BigGaming);

            return configuration.BrandMerchants.Where(x => x.CurrencyCode.IgnoreCaseEquals(currencyCode) && x.Status).First();
        }

        async ValueTask<Guid> IBigGamingServices.GetGameProviderId()
        {
            return await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.BigGaming).ConfigureAwait(false);
        }


        private BigGamingOrderStatus GetBGOrderStatus(GameActionType gameActionType) => gameActionType switch
        {
            GameActionType.PlaceBet => BigGamingOrderStatus.Running,
            GameActionType.WinBet => BigGamingOrderStatus.Win,
            GameActionType.TieBet => BigGamingOrderStatus.Tie,
            GameActionType.LoseBet => BigGamingOrderStatus.Lose,
            GameActionType.CancelTransaction => BigGamingOrderStatus.Cancel,
            _ => BigGamingOrderStatus.BetSlipDoesNotExist,
        };
    }
}
