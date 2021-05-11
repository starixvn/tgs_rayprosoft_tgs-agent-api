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
        private readonly IHashProvider _hashProvider;
        private readonly IVRGamePlayerCreate _gamePlayerCreate;

        public VRGameProviderWalletAction(
            IVRApiClient client,
            IHashProvider hashProvider,
            IGameProviderOperations gameProviderOperations,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderFundTransferTransactionRepository gameProviderFundTransferTransactionRepository,
            IWalletRepository walletRepository,
            IVRGamePlayerCreate gamePlayerCreate
            ) : base(playerOperations, gameProviderOperations, gameProviderFundTransferTransactionRepository, walletRepository)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _gamePlayerCreate = gamePlayerCreate ?? throw new ArgumentNullException(nameof(gamePlayerCreate));
        }

        public override ValueTask<(bool Checking, string OrderId)> CheckingOrderIdAsync(CachedPlayer player, string orderId)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<(bool Success, string OrderId, decimal Balance)> FundInAsync(CachedPlayer player, decimal amount)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<(bool Success, string OrderId, decimal Balance)> FundOutAsync(CachedPlayer player, decimal amount)
        {
            throw new NotImplementedException();
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

            var getBalance = await GetBalanceAsync(brandMerchant, player).ConfigureAwait(false);

            if (!getBalance.Success)
            {
                await _gamePlayerCreate.CreatePlayerAsync(player).ConfigureAwait(false);
                getBalance = await GetBalanceAsync(brandMerchant, player).ConfigureAwait(false);
            }

            return getBalance;
        }

        private async ValueTask<(bool Success, decimal Balance)> GetBalanceAsync(VRBrandMerchant brandMerchant, CachedPlayer player)
        {
            var data = new VRGetBalanceRequest()
            {
                PlayerCode = player.PlayerCode
            };
            var response = await _client.GetAPIAsync<VRGetBalanceRequest, VRGetBalanceResponse>(brandMerchant, GPAction.GetBalance, data).ConfigureAwait(false);

            if (string.IsNullOrEmpty(response.playerName) || response.balance == -1)
            {
                return new(false, 0);
            }
            return new(true, response.balance);
        }
    }
}
