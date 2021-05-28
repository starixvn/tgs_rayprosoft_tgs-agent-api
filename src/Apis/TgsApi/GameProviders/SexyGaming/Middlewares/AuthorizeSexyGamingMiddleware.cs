using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SexyGaming.Common;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Middlewares
{
    public sealed class AuthorizeSexyGamingMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        public AuthorizeSexyGamingMiddleware(
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations,
             ITokenProvider tokenProvider,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<SexyGamingConfiguration>(SettingConsts.GameProviderConfigurations.SexyGaming);

            var form = await context.Request.ReadFormAsync();
            var key = form.GetValue<string>(SexyGamingConsts.Parameters.Key);



            if (key.IgnoreCaseNotEquals(configuration.Cert))
            {
                throw new MissingTokenException($"Authentication encryption failed({key})");
            }

            var extension1 = form.GetValue<string>(SexyGamingConsts.Parameters.Extension1);
            var extensionDir = JsonSerializer.Deserialize<Dictionary<string, string>>(extension1);
            var loginRecord = await DecryptTokenAsync(extensionDir["token"]);

            var playerCode = loginRecord.PlayerCode;
            var platformType = loginRecord.PlatformType;

            var gameProviderId =
                await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.SexyGaming);
            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);
            context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);
            context.Items.Add(Consts.ContextItems.PlatformType, platformType);

            await next(context);
        }

        private async Task<CachedPlayerLoginRecord> DecryptTokenAsync(string token)
        {
            if (token.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }

            var loginRecordId = await _tokenProvider.DecryptAsync<Guid>(token);
            return await _playerOperations.GetPlayerLoginRecordAsync(loginRecordId);
        }
    }
}
