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
        private readonly IGameProviderOperations _gameProviderOperations;

        public KYGameProviderWalletAction(
            IKYApiClient client,
            IGameProviderOperations gameProviderOperations,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
            IWalletRepository walletRepository
            ) : base(playerOperations, gameProviderOperations, gameProviderFundTransferTransactionRepository, walletRepository)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
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

        public override async ValueTask<(bool Success, string OrderId, decimal? Balance)> FundInAsync(Guid transactionGameProviderId, CachedPlayer player, decimal amount)
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

            var dateTimeOffset = DateTimeOffset.UtcNow.AddHours(+8);
            var orderId = $"{agentId}{dateTimeOffset.ToString("yyyyMMddHHmmssfff")}{player.PlayerCode}";

            var data = new KYFnudRequest()
            {
                PlayerCode = player.PlayerCode,
                Amount = amount,
                OrderId = orderId
            };

            var response = await _client.FundAsync(brandMerchant, agentId, dateTimeOffset, GPAction.FundIn, data).ConfigureAwait(false);

            if (response.Data.Code != 0)
            {
                return (false, orderId, null);
            }

            Decimal.TryParse(response.Data.Balance, out decimal balance);

            return new(true, orderId, balance);
        }

        public override async ValueTask<(bool Success, string OrderId, decimal? Balance)> FundOutAsync(Guid transactionGameProviderId, CachedPlayer player, decimal amount)
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

            var dateTimeOffset = DateTimeOffset.UtcNow.AddHours(+8);
            var orderId = $"{agentId}{dateTimeOffset.ToString("yyyyMMddHHmmssfff")}{player.PlayerCode}";

            var data = new KYFnudRequest()
            {
                PlayerCode = player.PlayerCode,
                Amount = amount,
                OrderId = orderId
            };

            var response = await _client.FundAsync(brandMerchant, agentId, dateTimeOffset, GPAction.FundOut, data).ConfigureAwait(false);

            if (response.Data.Code != 0)
            {
                return (false, orderId, null);
            }

            Decimal.TryParse(response.Data.Balance, out decimal balance);

            return new(true, orderId, balance);
        }

        public override async ValueTask<(Guid GameProviderId, string GameProviderCode)> GetGameProviderAsync()
        {
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.KY).ConfigureAwait(false);
            return new(gameProviderId, Consts.GameProvider.KY);
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
            var dateTimeOffset = DateTimeOffset.UtcNow.AddHours(+8);

            var data = new KYGetBalanceRequest()
            {
                PlayerCode = player.PlayerCode
            };

            var response = await _client.GetBalanceAsync(brandMerchant, agentId, data).ConfigureAwait(false);

            if (response.Data.Code != 0)
            {
                return new(false, 0);
            }

            return new(true, response.Data.Balance);
        }
    }
}
