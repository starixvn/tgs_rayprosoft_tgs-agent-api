using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.JDB.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Middlewares
{
    public sealed class AuthorizeJDBMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IJsonSerializationProvider _json;
        private readonly ISettingOperations _settingOperations;
        private readonly ISymmetricCryptoProvider _cryptoProvider;

        public AuthorizeJDBMiddleware(
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IJsonSerializationProvider json,
            ISettingOperations settingOperations,
            ISymmetricCryptoProviderFactory cryptoProviderFactory)
        {
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _json = json;
            _settingOperations = settingOperations;
            _cryptoProvider = cryptoProviderFactory.GetProvider(SymmetricAlgorithmKind.AES);
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!context.Request.Form.TryGetValue("x", out var jsonEncrypted))
            {
                throw new ArgumentException();
            }
            
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<JDBConfiguration>(SettingConsts.GameProviderConfigurations.JDB);

            var jsonValue = await _cryptoProvider.DecryptStringAsync(jsonEncrypted, configuration.AESIv,
                configuration.AESKey);

            var baseRequest = _json.DeserializeFromString<BaseActionRequest>(jsonValue);
            var gameProviderId =
                await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.JDB);
            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(baseRequest.UserId);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
            
            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);
            context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            context.Items.Add(Consts.ContextItems.PlayerCode, baseRequest.UserId);
            
            context.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                {"x", jsonValue}
            });
            
            await next(context);
        }
    }
}