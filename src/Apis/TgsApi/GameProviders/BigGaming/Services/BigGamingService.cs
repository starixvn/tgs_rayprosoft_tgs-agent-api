using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.GameProviderDepositWithdrawal;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.BigGaming;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.BigGaming;
using TGS.Shared.Game.Models.BigGaming.Commons;
using TGS.Shared.Game.Models.BigGaming.Enums;
using TGS.Shared.Game.Models.BigGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public interface IBigGamingService
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType);
        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);
        ValueTask<decimal> GetPlayerBalanceAsync(string playerCode);
        ValueTask<BigGamingBrandMerchant> GetBrandMerchantAsync(string currencyCode);
        ValueTask<Guid> GetGameProviderIdAsync();
        ValueTask<BigGamingOrderStatus> GetOrderStatusAsync(Guid gameProviderId, string playerCode, string orderId);
        void CheckingSign(string sourceSign, string Sign);
        ValueTask<CachedPlayer> GetCachedPlayerAsync(string playerCode);
        ValueTask<Player> GetPlayerByCodeAsync(string playerCode);
        ValueTask<GameProviderDepositWithdrawalResponse> TransferAsync(Guid gameProviderId, string playerCode, string gameProviderTransactionId, decimal amount);
        ValueTask<CachedGame> GetCachedGameAsync(int gameId);
        ValueTask<CachedGame> GetCachedGameAsync(Guid gameProviderId, string externalGameId);
        ValueTask<BigGamingResponse> GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, CachedPlayer player, CachedGame game, string cultureCode);
        ValueTask<BigGamingResponse> CreatePlayerAsync(CachedPlayer player);
        ValueTask<bool> CreateAgentAsync(CachedPlayer player);
        ValueTask<Guid> GetBrandGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);
        string GetGameProviderCode();
        GameActionType GetGameAction(BigGamingOrderStatus betStatus);
        string GetBetType(string playId);
        decimal GetAmount(int externalGameId, string playId, decimal amount);
    }
    internal sealed class BigGamingService : IBigGamingService
    {
        private readonly ISettingOperations _settingOperations;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameProviderDepositWithdrawalService _gameProviderDepositWithdrawalService;
        private readonly IBigGamingApiClient _client;
        private readonly IBrandOperations _brandOperations;
        public BigGamingService(
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IPlayerOperations playerOperations,
            IGameProviderDepositWithdrawalService gameProviderDepositWithdrawalService,
            IBigGamingApiClient client,
            IBrandOperations brandOperations)
        {
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _gameProviderOperations = gameProviderOperations;
            _gameActionQueries = gameActionQueries;
            _playerOperations = playerOperations;
            _gameProviderDepositWithdrawalService = gameProviderDepositWithdrawalService ?? throw new ArgumentNullException(nameof(gameProviderDepositWithdrawalService));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
        }

        async ValueTask<GameActionExternalIds> IBigGamingService.GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
            => await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId).ConfigureAwait(false);

        async ValueTask<decimal> IBigGamingService.GetPlayerBalanceAsync(string playerCode)
        {
            var walletId = await _playerOperations.GetDefaultWalletIdByPlayerCodeAsync(playerCode).ConfigureAwait(false);

            return await _playerOperations.GetPlayerBalanceAsync(walletId, false).ConfigureAwait(false);
        }

        async ValueTask<GameProviderConfigurationData> IBigGamingService.GetGameProviderConfigurationDataAsync(
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

        async ValueTask<BigGamingOrderStatus> IBigGamingService.GetOrderStatusAsync(Guid gameProviderId, string playerCode, string orderId)
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

        async ValueTask<BigGamingBrandMerchant> IBigGamingService.GetBrandMerchantAsync(string currencyCode)
        {
            var configuration =
              await _settingOperations.GetValueFromJsonByKeyAsync<BigGamingConfiguration>(SettingConsts
                  .GameProviderConfigurations.BigGaming);

            return configuration.BrandMerchants.Where(x => x.CurrencyCode.IgnoreCaseEquals(currencyCode) && x.Status).First();
        }

        async ValueTask<Guid> IBigGamingService.GetGameProviderIdAsync()
        {
            return await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.BigGaming).ConfigureAwait(false);
        }

        void IBigGamingService.CheckingSign(string sourceSign, string Sign)
        {
            if (sourceSign.IgnoreCaseNotEquals(Sign))
            {
                throw new MissingTokenException($"Authentication encryption failed({sourceSign})");
            }
        }

        async ValueTask<CachedPlayer> IBigGamingService.GetCachedPlayerAsync(string playerCode)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode).ConfigureAwait(false);
            if (player == null)
            {
                throw new PlayerLoginExpiredException();
            }
            return player;
        }

        async ValueTask<Player> IBigGamingService.GetPlayerByCodeAsync(string playerCode)
        {
            var player = await _playerOperations.GetPlayerByCodeAsync(playerCode).ConfigureAwait(false);
            if (player == null)
            {
                throw new PlayerLoginExpiredException();
            }
            return player;
        }

        async ValueTask<GameProviderDepositWithdrawalResponse> IBigGamingService.TransferAsync(Guid gameProviderId, string playerCode, string gameProviderTransactionId, decimal amount)
        {
            var player = await _playerOperations.GetPlayerByCodeAsync(playerCode).ConfigureAwait(false);

            var request = new GameProviderDepositWithdrawalRequest()
            {
                GameProviderId = gameProviderId,
                BrandId = player.BrandId,
                PlayerCode = player.PlayerCode,
                PlayerWalletId = player.DefaultWalletId,
                GameProviderTransactionId = gameProviderTransactionId,
                Amount = amount
            };

            if (amount < 0)
            {
                return await _gameProviderDepositWithdrawalService.GameProviderDepositAsync(request).ConfigureAwait(false);

            }
            else
            {
                return await _gameProviderDepositWithdrawalService.GameProviderWithdrawalAsync(request).ConfigureAwait(false);

            }
        }

        async ValueTask<CachedGame> IBigGamingService.GetCachedGameAsync(int gameId)
        {
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.BigGaming).ConfigureAwait(false);
            return await _gameProviderOperations.GetCachedGameAsync(gameProviderId, gameId).ConfigureAwait(false);
        }

        async ValueTask<CachedGame> IBigGamingService.GetCachedGameAsync(Guid gameProviderId, string externalGameId)
        {
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId).ConfigureAwait(false);
            return games.FirstOrDefault();
        }

        async ValueTask<BigGamingResponse> IBigGamingService.GetGameUrlAsync(BigGamingBrandMerchant brandMerchant, CachedPlayer player, CachedGame game, string cultureCode) => game.Type
           switch
        {
            GameType.Fishing => await GetGameUrlFishingAsync(brandMerchant, player, game.ExternalId, cultureCode).ConfigureAwait(false),
            _ => await GetGameUrlLiveDealerAsync(brandMerchant, player, cultureCode).ConfigureAwait(false)
        };

        async ValueTask<BigGamingResponse> IBigGamingService.CreatePlayerAsync(CachedPlayer player)
        {
            var brandMerchant = await GetBrandMerchantAsync(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            var createPlayerRequest = new BigGamingCreatePlayerRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                LoginId = player.PlayerCode,
                Nickname = player.PlayerCode,
                AgentLoginId = player.BrandCode
            };

            return await _client.CreatePlayerAsync(brandMerchant, createPlayerRequest).ConfigureAwait(false);
        }

        async ValueTask<bool> IBigGamingService.CreateAgentAsync(CachedPlayer player)
        {
            var brandMerchant = await GetBrandMerchantAsync(player.DefaultWalletCurrencyCode).ConfigureAwait(false);
            var createAgentRequest = new BigGamingCreateAgentRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                LoginId = player.BrandCode,
                Password = brandMerchant.AgentLoginIdPassword
            };
            var responseCreateAgent = await _client.CreateAgentAsync(brandMerchant, createAgentRequest).ConfigureAwait(false);
            if (responseCreateAgent.Error != null)
            {
                throw new PlayerNotFoundException(player.PlayerCode);
            }
            else
            {
                return true;
            }
        }

        async ValueTask<Guid> IBigGamingService.GetBrandGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
        {
            return await _brandOperations.GetBrandGameProviderConfigurationIdAsync(brandId, gameProviderId);
        }

        GameActionType IBigGamingService.GetGameAction(BigGamingOrderStatus betStatus) => betStatus switch
        {
            BigGamingOrderStatus.Running => GameActionType.PlaceBet,
            BigGamingOrderStatus.Win => GameActionType.WinBet,
            BigGamingOrderStatus.Tie => GameActionType.TieBet,
            BigGamingOrderStatus.Lose => GameActionType.LoseBet,
            _ => GameActionType.CancelTransaction
        };

        string IBigGamingService.GetBetType(string playId)
           => $"0x{Convert.ToString(Convert.ToInt64(playId), 16).ToUpper()}";

        decimal IBigGamingService.GetAmount(int externalGameId, string playId, decimal amount)
        {
            if (externalGameId == 13)
            {
                var Odds = BigGamingConsts.BullBullOddsMapper.Where(x => x.Key.IgnoreCaseEquals(playId)).First().Value;
                return amount * Odds;
            }
            return amount;
        }

        private async ValueTask<BigGamingBrandMerchant> GetBrandMerchantAsync(string currencyCode)
        {
            var configuration =
            await _settingOperations.GetValueFromJsonByKeyAsync<BigGamingConfiguration>(SettingConsts
                .GameProviderConfigurations.BigGaming);

            return configuration.BrandMerchants.Where(x => x.CurrencyCode.IgnoreCaseEquals(currencyCode) && x.Status).First();

        }

        private async ValueTask<BigGamingResponse> GetGameUrlFishingAsync(BigGamingBrandMerchant brandMerchant, CachedPlayer player, string externalId, string cultureCode)
        {
            var request = new BigGamingGetGameUrlFishingRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                LoginId = player.PlayerCode,
                Locale = cultureCode,
                IsMobileUrl = 0,
                IsHttpsUrl = 0,
                ReturnUrl = string.Empty,
                FromIp = string.Empty,
                GameType = externalId

            };
            return await _client.GetGameUrlAsync(brandMerchant, request).ConfigureAwait(false);
        }

        private async ValueTask<BigGamingResponse> GetGameUrlLiveDealerAsync(BigGamingBrandMerchant brandMerchant, CachedPlayer player, string cultureCode)
        {
            var request = new BigGamingGetGameUrlRequest()
            {
                Random = GuidExtensions.NewSequentialGuid().ToString(),
                Sn = brandMerchant.SecretCode,
                LoginId = player.PlayerCode,
                Locale = cultureCode,
                IsMobileUrl = 0,
                IsHttpsUrl = 0,
                ReturnUrl = string.Empty,
                FromIp = string.Empty,
            };
            return await _client.GetGameUrlAsync(brandMerchant, request).ConfigureAwait(false);
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

        string IBigGamingService.GetGameProviderCode()
        {
            return Consts.GameProvider.BigGaming;
        }
    }
}
