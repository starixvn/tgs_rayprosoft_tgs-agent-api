using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Middlewares
{
    public sealed class AuthorizePragmaticPlayMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPragmaticPlayHashProvider _hashProvider;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;

        public AuthorizePragmaticPlayMiddleware(
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations,
            ITokenProvider tokenProvider,
            IPragmaticPlayHashProvider hashProvider)
        {
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _settingOperations = settingOperations;
            _tokenProvider = tokenProvider;
            _hashProvider = hashProvider;
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var form = await context.Request.ReadFormAsync();

            var configuration = await _settingOperations
                .GetValueFromJsonByKeyAsync<PragmaticPlayConfiguration>(SettingConsts.GameProviderConfigurations
                    .PragmaticPlay);

            var providerId = form.GetValue<string>(PragmaticPlayConsts.Parameters.ProviderId);
            if (configuration.ProviderId.IgnoreCaseNotEquals(providerId))
            {
                throw new InvalidCredentialsException("Invalid providerId");
            }

            var bypassHashVerify = form.GetValue<bool>(PragmaticPlayConsts.Parameters.BypassHashVerify);
            if (!bypassHashVerify)
            {
                var hash = form.GetValue<string>(PragmaticPlayConsts.Parameters.Hash);
                var computedHash = _hashProvider.ComputeHash(form, configuration.SecretKey);

                if (hash.IgnoreCaseNotEquals(computedHash))
                {
                    throw new InvalidCredentialsException("Invalid hash");
                }
            }

            var token = form.GetValue<string>(PragmaticPlayConsts.Parameters.Token);
            var loginRecord = await DecryptTokenAsync(token);
            var playerCode = loginRecord.PlayerCode;
            var platformType = loginRecord.PlatformType;

            var gameProviderId =
                await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.PragmaticPlay);
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