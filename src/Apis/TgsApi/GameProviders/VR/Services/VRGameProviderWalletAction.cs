using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Commons.Services.GameProviderWalletAction;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.GameProviders.Settings.VR;
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
        }

        public override async ValueTask<(bool Checking, string OrderId)> CheckingOrderIdAsync(CachedPlayer player, string orderId)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<VRConfiguration>(SettingConsts
                    .GameProviderConfigurations.VR);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }

            var data = new VRFundCheckingRequest()
            {
                StartTime = DateTimeOffset.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
                endTime = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                SerialNumber = orderId,
                PlayerCode = player.PlayerCode
            };

            var response = await _client.GetAPIAsync<VRFundCheckingRequest, VRFundCheckingResponse>(brandMerchant, GPAction.FundChecking, data);
            if (response.TotalRecords == 0)
            {
                return new(false, string.Empty);
            }
            return new(true, response.Records.First().SerialNumber);
        }

        public async override ValueTask<(bool Success, string OrderId, decimal Balance)> FundInAsync(CachedPlayer player, decimal amount)
        {
            var configuration =
              await _settingOperations.GetValueFromJsonByKeyAsync<VRConfiguration>(SettingConsts
                  .GameProviderConfigurations.VR);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }


            var date = DateTimeOffset.UtcNow;
            var data = new VRFnudRequest()
            {
                SerialNumber = date.AddHours(+8).ToUnixTimeSeconds().ToString(),
                PlayerCode = player.PlayerCode,
                Type = 1,
                Amount = 1,
                CreateTime = date.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            var response = await _client.GetAPIAsync<VRFnudRequest, VRFnudResponse>(brandMerchant, GPAction.GetBalance, data).ConfigureAwait(false);
            if (response.State != 0)
            {
                return new() { Success = false, Balance = 0, OrderId = string.Empty };
            }
            return new() { Success = true, Balance = response.Balance, OrderId = response.SerialNumber };
        }

        public async override ValueTask<(bool Success, string OrderId, decimal Balance)> FundOutAsync(CachedPlayer player, decimal amount)
        {
            var configuration =
              await _settingOperations.GetValueFromJsonByKeyAsync<VRConfiguration>(SettingConsts
                  .GameProviderConfigurations.VR);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }


            var date = DateTimeOffset.UtcNow;
            var data = new VRFnudRequest()
            {
                SerialNumber = date.AddHours(+8).ToUnixTimeSeconds().ToString(),
                PlayerCode = player.PlayerCode,
                Type = 0,
                Amount = 1,
                CreateTime = date.ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            var response = await _client.GetAPIAsync<VRFnudRequest, VRFnudResponse>(brandMerchant, GPAction.GetBalance, data).ConfigureAwait(false);
            if (response.State != 0)
            {
                return new() { Success = false, Balance = 0, OrderId = string.Empty };
            }
            return new() { Success = true, Balance = response.Balance, OrderId = response.SerialNumber };
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
            var response = await _client.GetAPIAsync<VRGetBalanceRequest, VRGetBalanceResponse>(brandMerchant, GPAction.GetBalance, data).ConfigureAwait(false);

            if (string.IsNullOrEmpty(response.playerName) || response.balance == -1)
            {
                if (await _gamePlayerCreate.CreatePlayerAsync(player).ConfigureAwait(false))
                {
                    response = await _client.GetAPIAsync<VRGetBalanceRequest, VRGetBalanceResponse>(brandMerchant, GPAction.GetBalance, data).ConfigureAwait(false);
                }
            }

            if (string.IsNullOrEmpty(response.playerName) || response.balance == -1)
            {
                return new(false, 0);

            }

            return new(true, response.balance);
        }
    }
}
