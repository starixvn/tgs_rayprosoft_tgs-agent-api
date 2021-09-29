using System;
using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.Card365.Middlewares
{
    public class AuthorizeCard365Middleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;

        public AuthorizeCard365Middleware(
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ITokenProvider tokenProvider)
        {
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _gameProviderOperations =
                gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var data = await context.Request.ReadAsStringAsync();
            using var document = JsonDocument.Parse(data);

            if (!document.RootElement.TryGetProperty("key", out var element))
            {
                throw new InvalidCredentialException("Missing key parameter");
            }

            var key = element.GetString();
            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<Card365Configuration>(SettingConsts
                    .GameProviderConfigurations.Card365);

            if (configuration.Cert.IgnoreCaseNotEquals(key))
            {
                throw new InvalidCredentialException("Invalid cert");
            }

            var gameProviderId = await _gameProviderOperations
                .GetGameProviderIdByCodeAsync(Consts.GameProvider.Card365);
            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);

            if (!document.RootElement.TryGetProperty("userId", out element))
            {
                await next(context);
                return;
            }

            var playerCode = element.GetString();
            if (playerCode.IsNullOrEmpty())
            {
                await next(context);
                return;
            }

            context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);

            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);

            if (!document.RootElement.TryGetProperty("extension3", out element))
            {
                await next(context);
                return;
            }

            var loginRecord = await DecryptTokenAsync(element.GetString());
            var platformType = loginRecord?.PlatformType ?? PlatformType.Desktop;

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