using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Shared.Game.ApiClients.BigGaming;
using TGS.Shared.Game.Models.BigGaming.Common;
using TGS.Shared.Game.Models.BigGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Services
{
    public interface IBigGamingGamePlayerCreate
    {
        ValueTask<bool> CreatePlayerAsync(CachedPlayer player);
    }
    sealed class BigGamingGamePlayerCreate : IBigGamingGamePlayerCreate
    {
        private readonly IBigGamingApiClient _client;
        private readonly ISettingOperations _settingOperations;
        private readonly IBigGamingServices _bigGamingServices;

        public BigGamingGamePlayerCreate(
            IBigGamingApiClient client,
            ISettingOperations settingOperations,
            IBigGamingServices bigGamingServices
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _bigGamingServices = bigGamingServices ?? throw new ArgumentNullException(nameof(bigGamingServices));
        }

        public async ValueTask<bool> CreatePlayerAsync(CachedPlayer player)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<BigGamingConfiguration>(SettingConsts
                    .GameProviderConfigurations.BigGaming);

            var response = await CreatePlayerAsync(configuration, player);

            if (response.Error != null)
            {
                if (response.Error.Code == "2610")
                {
                    await CreateAgentAsync(configuration, player);
                    response = await CreatePlayerAsync(configuration, player);
                    if (response.Error != null)
                    {
                        throw new PlayerNotFoundException(player.PlayerCode);
                    }

                }
                else
                {
                    throw new PlayerNotFoundException(player.PlayerCode);
                }
            }
            return true;
        }

        private async Task<bool> CreateAgentAsync(BigGamingConfiguration configuration, CachedPlayer player)
        {
            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);
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

        private async Task<BigGamingResponse> CreatePlayerAsync(BigGamingConfiguration configuration, CachedPlayer player)
        {
            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);
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
    }
}
