using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.GameProviders.Repositories;
using TGS.Apis.TgsApi.Commons.Services.GameProviderWalletAction;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.WalletService;
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.Models.KY.Commons;
using TGS.Shared.Game.Models.KY.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.KY.Services
{
    public class KYGameProviderWalletAction : BaseGameProviderWalletAction
    {
        private readonly string _gameProvider = Consts.GameProvider.KY;
        private readonly string _gameProviderConfigurations = SettingConsts.GameProviderConfigurations.KY;
        private readonly IKYApiClient _client;
        private readonly ISettingOperations _settingOperations;
        private readonly IHashProvider _hashProvider;
        private readonly IGameProviderOperations _gameProviderOperations;

        public KYGameProviderWalletAction(
            IKYApiClient client,
            IHashProvider hashProvider,
            IGameProviderOperations gameProviderOperations,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
            IWalletRepository walletRepository
            ) : base(playerOperations, gameProviderOperations, gameProviderFundTransferTransactionRepository, walletRepository)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
        }

        public override async ValueTask<bool> CheckingAmountAsync(CachedPlayer player, decimal amount)
        {
            var configuration =
                  await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                      .GameProviderConfigurations.KY);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }

            if (configuration.AmountMaximum < amount)
            {
                return false;
            }

            if (amount.Truncate(Convert.ToInt32(configuration.AmountFormat.Split(',')[1])) < amount)
            {
                return false;
            }

            return true;
        }

        public override async ValueTask<(bool Checking, string OrderId)> CheckingOrderIdAsync(CachedPlayer player, string orderId)
        {
            var configuration =
                 await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                     .GameProviderConfigurations.KY);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }

            var agentId = brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(player.DefaultWalletCurrencyCode) && x.Status).FirstOrDefault().AgentId;

            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var timestamp = date.ToUnixTimeMilliseconds();


            var data = new KYFundCheckingRequest()
            {
                Action = KYUtility.GetAction(GPAction.CreatePlayer) ?? -1,
                OrderId = orderId
            };

            var dataPath = KYUtility.GetPath(data);
            var param = KYUtility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _hashProvider.Hash(plainText, HashAlgorithmKind.Md5);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await _client.GetAPIAsync<KYFundCheckingResponse>(brandMerchant.ApiUrl, apiKYRequest).ConfigureAwait(false);

            var checking = false;
            if (response.Data.Code == 0 && response.Data.Status == 0)
            {
                checking = true;
            }

            return new() { Checking = checking, OrderId = (checking) ? orderId : null };

        }

        public override async ValueTask<(bool Success, string OrderId, decimal Balance)> FundInAsync(CachedPlayer player, decimal amount)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                    .GameProviderConfigurations.KY);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }

            var agentId = brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(player.DefaultWalletCurrencyCode) && x.Status).FirstOrDefault().AgentId;

            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var timestamp = date.ToUnixTimeMilliseconds();
            var orderId = $"{agentId}{date.ToString("yyyyMMddHHmmss")}{player.PlayerCode}";


            var data = new KYFnudRequest()
            {
                Action = KYUtility.GetAction(GPAction.FundIn) ?? -1,
                PlayerCode = player.PlayerCode,
                Amount = amount,
                OrderId = orderId
            };
            var dataPath = KYUtility.GetPath(data);
            var param = KYUtility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _hashProvider.Hash(plainText, HashAlgorithmKind.Md5);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await _client.GetAPIAsync<KYFnudResponse>(brandMerchant.ApiUrl, apiKYRequest).ConfigureAwait(false);

            var balance = default(decimal);
            var success = false;
            if (response.Data.Code == 0)
            {
                Decimal.TryParse(response.Data.Balance, out balance);
                success = true;
            }


            return new() { Success = success, Balance = balance, OrderId = orderId };
        }

        public override async ValueTask<(bool Success, string OrderId, decimal Balance)> FundOutAsync(CachedPlayer player, decimal amount)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                    .GameProviderConfigurations.KY);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }
            var agentId = brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(player.DefaultWalletCurrencyCode) && x.Status).FirstOrDefault().AgentId;

            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var timestamp = date.ToUnixTimeMilliseconds();
            var orderId = $"{agentId}{date.ToString("yyyyMMddHHmmss")}{player.PlayerCode}";


            var data = new KYFnudRequest()
            {
                Action = KYUtility.GetAction(GPAction.FundOut) ?? -1,
                PlayerCode = player.PlayerCode,
                Amount = amount,
                OrderId = orderId
            };
            var dataPath = KYUtility.GetPath(data);
            var param = KYUtility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _hashProvider.Hash(plainText, HashAlgorithmKind.Md5);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await _client.GetAPIAsync<KYFnudResponse>(brandMerchant.ApiUrl, apiKYRequest).ConfigureAwait(false);

            var balance = default(decimal);
            var success = false;
            if (response.Data.Code == 0)
            {
                Decimal.TryParse(response.Data.Balance, out balance);
                success = true;
            }


            return new() { Success = success, Balance = balance, OrderId = orderId };
        }

        public override async ValueTask<(Guid GameProviderId, string GameProviderCode)> GetGameProviderAsync()
        {
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(_gameProvider).ConfigureAwait(false);
            return new(gameProviderId, _gameProvider);
        }

        public override async ValueTask<(bool Success, decimal Balance)> GetGameProviderBalanceAsync(CachedPlayer player)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                    .GameProviderConfigurations.KY);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }

            var agentId = brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(player.DefaultWalletCurrencyCode) && x.Status).FirstOrDefault().AgentId;
            var date = DateTimeOffset.UtcNow.AddHours(+8);
            var timestamp = date.ToUnixTimeMilliseconds();
            var orderId = $"{agentId}{date.ToString("yyyyMMddHHmmss")}{player.PlayerCode}";

            var data = new KYGetBalanceRequest()
            {
                Action = KYUtility.GetAction(GPAction.GetBalance) ?? -1,
                PlayerCode = player.PlayerCode
            };
            var dataPath = KYUtility.GetPath(data);
            var param = KYUtility.AesEncrypt(dataPath, brandMerchant.Deskey, true);
            var plainText = $"{agentId}{timestamp}{brandMerchant.Md5key}";
            var key = _hashProvider.Hash(plainText, HashAlgorithmKind.Md5);

            var apiKYRequest = new KYApiRequest()
            {
                Agent = agentId,
                Timestamp = timestamp,
                Param = param,
                Key = key
            };

            var response = await _client.GetAPIAsync<KYGetBalanceResponse>(brandMerchant.ApiUrl, apiKYRequest).ConfigureAwait(false);

            if (response.Data.Code != 0)
            {
                return new(false, 0);
            }

            return new(true, response.Data.Balance);
        }
    }
}
