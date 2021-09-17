using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.GameProviderWalletAction;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.GameProviders.Repositories;
using TGS.Services.Operations;
using TGS.Services.WalletService;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.ApiClients.VR;
using TGS.Shared.Game.Models.VR.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.VR.Services
{
    public class VRGameProviderWalletAction : BaseGameProviderWalletAction
    {
        private readonly IVRApiClient _client;
        private readonly ISettingOperations _settingOperations;
        private readonly IVRGamePlayerCreate _gamePlayerCreate;
        private readonly IGameProviderOperations _gameProviderOperations;

        public VRGameProviderWalletAction(
            IVRApiClient client,
            IGameProviderOperations gameProviderOperations,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
            IWalletRepository walletRepository,
            IVRGamePlayerCreate gamePlayerCreate
            ) : base(playerOperations, gameProviderOperations, gameProviderFundTransferTransactionRepository, walletRepository)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _gamePlayerCreate = gamePlayerCreate ?? throw new ArgumentNullException(nameof(gamePlayerCreate));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
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

        public async override ValueTask<(bool Success, string OrderId, decimal? Balance)> FundInAsync(Guid transactionGameProviderId, CachedPlayer player, decimal amount)
        {
            var configuration =
              await _settingOperations.GetValueFromJsonByKeyAsync<VRConfiguration>(SettingConsts
                  .GameProviderConfigurations.VR);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }

            var orderId = $"{transactionGameProviderId.ToString("N")}";
            var date = DateTimeOffset.UtcNow;
            var data = new VRFnudRequest()
            {
                SerialNumber = orderId,
                PlayerCode = player.PlayerCode,
                Type = 1,
                Amount = amount,
                CreateTime = date.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            var response = await _client.FundAsync(brandMerchant, GPAction.FundIn, data).ConfigureAwait(false);
            if (response.State != 0)
            {
                return (false, orderId, null);
            }
            return (true, response.SerialNumber, response.Balance);
        }

        public async override ValueTask<(bool Success, string OrderId, decimal? Balance)> FundOutAsync(Guid transactionGameProviderId, CachedPlayer player, decimal amount)
        {
            var configuration =
              await _settingOperations.GetValueFromJsonByKeyAsync<VRConfiguration>(SettingConsts
                  .GameProviderConfigurations.VR);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }

            var orderId = $"{transactionGameProviderId.ToString("N")}";
            var date = DateTimeOffset.UtcNow;
            var data = new VRFnudRequest()
            {
                SerialNumber = orderId,
                PlayerCode = player.PlayerCode,
                Type = 0,
                Amount = amount,
                CreateTime = date.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            var response = await _client.FundAsync(brandMerchant, GPAction.FundOut, data).ConfigureAwait(false);
            if (response.State != 0)
            {
                return (false, orderId, null);
            }
            return (true, response.SerialNumber, response.Balance);
        }

        public override async ValueTask<(Guid GameProviderId, string GameProviderCode)> GetGameProviderAsync()
        {
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.VR).ConfigureAwait(false);
            return new(gameProviderId, Consts.GameProvider.VR);
        }

        public override async ValueTask<(bool Success, decimal Balance)> GetGameProviderBalanceAsync(CachedPlayer player)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<VRConfiguration>(SettingConsts
                    .GameProviderConfigurations.VR);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }

            var data = new VRGetBalanceRequest()
            {
                PlayerCode = player.PlayerCode
            };
            var response = await _client.GetBalanceAsync(brandMerchant, data).ConfigureAwait(false);

            if (string.IsNullOrEmpty(response.PlayerName) || response.Balance == -1)
            {
                if (await _gamePlayerCreate.CreatePlayerAsync(player).ConfigureAwait(false))
                {
                    response = await _client.GetBalanceAsync(brandMerchant, data).ConfigureAwait(false);
                }
            }

            if (string.IsNullOrEmpty(response.PlayerName) || response.Balance == -1)
            {
                return new(false, 0);

            }

            return new(true, response.Balance);
        }
    }
}
