using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
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
        private readonly ISymmetricCryptoProvider _cryptoProvider;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IJsonSerializationProvider _json;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

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
            if (!context.Request.Form.TryGetValue("x", out var encrypted))
            {
                throw new ArgumentException("Invalid argument 'x'");
            }

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<JDBConfiguration>(SettingConsts
                    .GameProviderConfigurations.JDB);

            var jsonValue = await DecryptRequestStringToJsonAsync(encrypted, configuration.AESIv, configuration.AESKey);
            var baseRequest = _json.DeserializeFromString<BaseActionRequest>(jsonValue);

            var gameProviderId =
                await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.JDB);
            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(baseRequest.UserId);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);
            context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            context.Items.Add(Consts.ContextItems.PlayerCode, baseRequest.UserId);

            context.Request.Form = new FormCollection(new()
            {
                ["action"] = baseRequest.ActionType.ToString(),
                ["value"] = jsonValue
            });

            await next(context);
        }

        private async ValueTask<string> DecryptRequestStringToJsonAsync(string encrypted,
            string aseIv, string aesKey)
        {
            // var value = encrypted.Replace("-", "+").Replace("_", "/");
            // var paddingCount = (4 - value.Length % 4) % 4;
            // for (var i = 0; i < paddingCount; i++)
            // {
            //     value += "=";
            // }
            
            var t = WebEncoders.Base64UrlDecode(encrypted);
            var value = Convert.ToBase64String(t);

            var json = await _cryptoProvider.DecryptStringAsync(value, aseIv, aesKey,
                padding: PaddingMode.None);

            json = json.Replace("", string.Empty);
            json = json.Replace("", string.Empty);
            json = json.Replace("", string.Empty);
            json = json.Replace("", string.Empty);
            json = json.Replace("", string.Empty);
            return json.Replace("\0", string.Empty);
        }
    }
}