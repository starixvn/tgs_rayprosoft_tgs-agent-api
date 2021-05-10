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
        private readonly IKYApiClient _client;
        private readonly ISettingOperations _settingOperations;
        private readonly IHashProvider _hashProvider;

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
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
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
                Action = KYUtility.GetAction(GPAction.FnudIn) ?? -1,
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
                Action = KYUtility.GetAction(GPAction.FnudOut) ?? -1,
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

        public override async ValueTask<decimal?> GetGameProviderBalanceAsync(CachedPlayer player)
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

            var data = new KYGetBlaanceRequest()
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

            var response = await _client.GetAPIAsync<KYGetBlaanceResponse>(brandMerchant.ApiUrl, apiKYRequest).ConfigureAwait(false);

            var balance = default(decimal?);
            if (response.Data.Code == 0)
            {
                balance = response.Data.Balance;
            }

            return balance;
        }
    }
}
