using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.Skywind.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Middlewares
{
    public sealed class AuthorizeSkywindMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ISkywindServices _services;
        public AuthorizeSkywindMiddleware(
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations,
            IJsonSerializationProvider jsonSerializationProvider,
            ISkywindServices services)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _services = services ?? throw new ArgumentNullException(nameof(services));
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SkywindConfiguration>(SettingConsts.GameProviderConfigurations.Skywind);
            var gameProviderId =
                 await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.Skywind);
            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);


            var value = await context.Request.ReadAsStringAsync();
            var keyValuePairs = _jsonSerializationProvider.DeserializeFromString<Dictionary<string, object>>(_jsonSerializationProvider.SerializeToString((JsonDocument.Parse(value))));

            if (keyValuePairs["merch_id"].ToString().IgnoreCaseNotEquals(configuration.MerchId) || keyValuePairs["merch_pwd"].ToString().IgnoreCaseNotEquals(configuration.MerchPwd))
            {
                throw new InvalidCredentialsException("empty token");
            }

            if (context.Request.Path.Value.IgnoreCaseContains("api/sw/get_balance") || context.Request.Path.Value.IgnoreCaseContains("api/sw/debit"))
            {
                //var loginRecord = await _services.DecryptTokenAsync(keyValuePairs["cust_session_id"].ToString());
                var playerCode = "4ucmqplxb029";//loginRecord.PlayerCode;
                var platformType = PlatformType.Desktop;//loginRecord.PlatformType;
                var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
                var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

                context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
                context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);
                context.Items.Add(Consts.ContextItems.PlatformType, platformType);
            }
            else
            {
                if (!context.Request.Path.Value.IgnoreCaseContains("api/sw/ping"))
                {

                    var playerCode = "4ucmqplxb029" //keyValuePairs["cust_id"].ToString();
                    var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
                    var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
                    context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
                    context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);
                }
            }
            await next(context);
        }
    }
}
