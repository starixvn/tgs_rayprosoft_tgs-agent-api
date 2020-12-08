// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

using System.Threading.Tasks;
using TGS.Apis.TgsApi.GameModels.SBOBET.Enums;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Clients;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Messages.Pluggables;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Services
{
    public sealed class SBOBETPlayerAction : IPlayerAction
    {
        private readonly ISBOApiClient _client;
        private readonly ISettingOperations _settingOperations;

        public SBOBETPlayerAction(ISettingOperations settingOperations, ISBOApiClient client)
        {
            _settingOperations = settingOperations;
            _client = client;
        }

        async Task<PlayerKickResponse> IPlayerAction.KickAsync(PlayerKickRequest request)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts
                    .GameProviderConfigurations.SBOBET);

            var response = await _client.LogoutAsync(configuration.ApiUrl,
                new()
                {
                    CompanyKey = configuration.CompanyKey,
                    PlayerCode = request.PlayerCode
                });

            return response.Error.Code != ServiceApiError.None
                ? new PlayerKickResponse
                {
                    Error = (int) TgsApiError.ProviderApiError,
                    ErrorMessage = response.Error.Message
                }
                : new();
        }
    }
}
