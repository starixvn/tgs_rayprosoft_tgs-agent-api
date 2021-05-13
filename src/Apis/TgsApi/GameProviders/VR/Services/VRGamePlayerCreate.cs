using System;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.ApiClients.VR;
using TGS.Shared.Game.Models.VR.Commons;
using TGS.Shared.Game.Models.VR.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.VR.Services
{
    public interface IVRGamePlayerCreate
    {
        ValueTask<bool> CreatePlayerAsync(CachedPlayer player);
    }
    sealed class VRGamePlayerCreate : IVRGamePlayerCreate
    {
        private readonly IVRApiClient _client;
        private readonly ISettingOperations _settingOperations;

        public VRGamePlayerCreate(
            IVRApiClient client
            , ISettingOperations settingOperations)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
        }

        public async ValueTask<bool> CreatePlayerAsync(CachedPlayer player)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<VRConfiguration>(SettingConsts
                    .GameProviderConfigurations.VR);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }

            var data = new VRCreatePlayerRequest()
            {
                PlayerCode = player.PlayerCode
            };
            var response = await _client.GetAPIAsync<VRCreatePlayerRequest, VRErrorCodeResponse>(brandMerchant, GPAction.CreatePlayer, data).ConfigureAwait(false);

            if (response.errorCode != 0)
            {
                throw new PlayerNotFoundException(player.PlayerCode);
            }

            return true;
        }
    }
}
