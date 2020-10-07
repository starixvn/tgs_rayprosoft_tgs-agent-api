using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Middlewares
{
    public sealed class AuthorizeSBOMiddleware : IMiddleware
    {
        private readonly ISettingOperations _settingOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ILicenseeOperations _licenseeOperations;

        public AuthorizeSBOMiddleware(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            ILicenseeOperations licenseeOperations)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _licenseeOperations = licenseeOperations;
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var configuration = await _settingOperations
                .GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts.GameProviderConfigurations.SBOBET);

            context.Request.EnableBuffering();
            
            var request = await context.Request.ReadFromJsonAsync<BaseRequest>();
            
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            if (request.CompanyKey.IsNullOrEmpty() || request.CompanyKey.NotEquals(configuration.CompanyKey))
            {
                throw new InvalidCredentialsException($"Invalid company key: {request.CompanyKey}");
            }

            var playerCode = request.PlayerCode;
            var loginRecord = await _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(playerCode);

            if (loginRecord == null)
            {
                throw new PlayerLoginExpiredException(playerCode);
            }

            var licenseeId = await _playerOperations.GetLicenseeIdByPlayerCodeAsync(playerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProvider.SBOBET);
            var lgpc = await _licenseeOperations.GetLicenseeGameProviderConfigurationAsync(licenseeId, gameProviderId);

            context.Items.Add(ContextItems.LicenseeId, licenseeId);
            context.Items.Add(ContextItems.GameProviderId, gameProviderId);
            context.Items.Add(ContextItems.GameProviderConfigurationId, lgpc.GameProviderConfigurationId);
            context.Items.Add(ContextItems.PlatformType, loginRecord.PlatformType);

            await next(context);
        }
    }
}