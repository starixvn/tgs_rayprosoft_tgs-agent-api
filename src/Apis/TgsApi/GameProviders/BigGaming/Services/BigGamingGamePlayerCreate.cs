using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
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
        private readonly IHashProvider _hashProvider;

        public BigGamingGamePlayerCreate(
            IBigGamingApiClient client,
            ISettingOperations settingOperations,
            IHashProvider _hashProvider
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            this._hashProvider = _hashProvider ?? throw new ArgumentNullException(nameof(_hashProvider));
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
            var random = GuidExtensions.NewSequentialGuid().ToString();
            var sign = _hashProvider.Hash($"{random }{configuration.SecretCode}{player.BrandCode}{configuration.SecretKey}", HashAlgorithmKind.Md5);
            var createAgentRequest = new BGCreateAgentRequest()
            {
                Random = random,
                Sign = sign,
                Sn = configuration.SecretCode,
                LoginId = player.BrandCode,
                Password = configuration.AgentLoginIdPassword
            };
            var responseCreateAgent = await _client.CreateAgentAsync(configuration.ApiUrl, createAgentRequest).ConfigureAwait(false);
            if (responseCreateAgent.Error != null)
            {
                throw new PlayerNotFoundException(player.PlayerCode);
            }
            else
            {
                return true;
            }
        }

        private async Task<BGBaseResponse> CreatePlayerAsync(BigGamingConfiguration configuration, CachedPlayer player)
        {
            var random = GuidExtensions.NewSequentialGuid().ToString();
            var digest = _hashProvider.Hash($"{random }{configuration.SecretCode}{BigGamingUtility.GetSecretCode(configuration.AgentLoginIdPassword)}", HashAlgorithmKind.Md5);
            var createPlayerRequest = new BGCreatePlayerRequest()
            {
                Random = random,
                Digest = digest,
                Sn = configuration.SecretCode,
                LoginId = player.PlayerCode,
                Nickname = player.PlayerCode,
                AgentLoginId = player.BrandCode
            };

            return await _client.CreatePlayerAsync(configuration.ApiUrl, createPlayerRequest).ConfigureAwait(false);
        }
    }
}
