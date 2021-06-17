using System;
using System.Threading.Tasks;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.Skywind;
using TGS.Shared.Game.Models.Skywind.Outgoing;
using TGS.Shared.Game.Models.Skywind.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Infrastructures.Security;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public sealed class SkywindGameInitializer : IGameInitializer
    {
        private readonly ISkywindApiClient _client;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISkywindServices _skywindServices;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;

        public SkywindGameInitializer(
            ISkywindApiClient client,
            IPlayerOperations playerOperations,
            ISkywindServices skywindServices,
            IJsonSerializationProvider jsonSerializationProvider,
            ISettingOperations settingOperations,
            ITokenProvider tokenProvider
            )
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _skywindServices = skywindServices ?? throw new ArgumentNullException(nameof(skywindServices));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var token = request.Token;
            var loginTokenId = token.LoginRecordId;
            var encryptedToken = await _tokenProvider.EncryptAsync(loginTokenId);
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);
            var configuration = await _settingOperations.GetValueFromJsonByKeyAsync<SkywindConfiguration>(SettingConsts.GameProviderConfigurations.Skywind);
            var data = new SkywindGameUrlRequest()
            {
                PlayerCode = player.PlayerCode,
                GameCode = "sw_bac",
                Ticket = encryptedToken,
            };

            var url = default(string);
            var responseGameUrl = await _client.GetGameUrlAsync(configuration, data).ConfigureAwait(false);
            if (responseGameUrl.Code.ToString().IgnoreCaseNotEquals(((int)GameApiError.Success).ToString()))
            {
                throw new GameProviderApiErrorException(_jsonSerializationProvider.SerializeToString(responseGameUrl));
            }
            else
            {
                url = responseGameUrl.Url;
            }
            return new() { Url = url };
        }
    }
}