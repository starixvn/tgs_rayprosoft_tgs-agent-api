using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.Evolution.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Evolution.Commons;
using TGS.Shared.Game.Models.Evolution.Enums;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Middlewares
{
    public sealed class AuthorizeEvolutionMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly IEvolutionServices _services;
        private readonly IEvolutionRedisCacheService _redisCacheService;

        public AuthorizeEvolutionMiddleware(
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations,
            IJsonSerializationProvider jsonSerializationProvider,
            IEvolutionServices services,
            IEvolutionRedisCacheService redisCacheService)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            _redisCacheService = redisCacheService ?? throw new ArgumentNullException(nameof(redisCacheService));
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {

            string authToken = context.Request.Query["authToken"];
            var config = await _services.GetConfiguration().ConfigureAwait(false);

            if (authToken.IgnoreCaseNotEquals(config.OwAuthToken))
            {
                throw new InvalidCaptchaException(GameApiError.InvalidTokenId.Description());
            }

            var body = await context.Request.ReadAsStringAsync();
            var baseRequest = _jsonSerializationProvider.DeserializeFromString<EvolutionBaseRequest>(body);

            var gameProviderId = await _services.GetGameProviderIdAsync();
            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);

            if (context.Request.Path.Value.IgnoreCaseContains("evo/api/balance")
               || context.Request.Path.Value.IgnoreCaseContains("evo/api/debit")
               || context.Request.Path.Value.IgnoreCaseContains("evo/api/credit")
               || context.Request.Path.Value.IgnoreCaseContains("evo/api/cancel")
               || context.Request.Path.Value.IgnoreCaseContains("evo/api/check"))
            {
                var playerInfo = default(EvolutionGetPlayerInfo);

                var playerCode = await _redisCacheService.GetPlayerCodeAsync(baseRequest.Sid).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(playerCode))
                {
                    playerInfo = await CheckingUserIdAsync(gameProviderId, playerCode).ConfigureAwait(false);
                }
                else
                {
                    playerInfo = await CheckingTokenAsync(gameProviderId, baseRequest.Sid).ConfigureAwait(false);
                }

                context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, playerInfo.GameProviderConfigurationId);
                context.Items.Add(Consts.ContextItems.PlayerCode, playerInfo.PlayerCode);
                context.Items.Add(Consts.ContextItems.PlatformType, playerInfo.PlatformType);
            }

            await next(context);
        }

        private async Task<EvolutionGetPlayerInfo> CheckingTokenAsync(Guid gameProviderId, string token)
        {
            var loginRecord = await _services.DecryptTokenAsync(token);

            var playerCode = loginRecord.PlayerCode;
            var platformType = loginRecord.PlatformType;
            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            return new EvolutionGetPlayerInfo()
            {
                GameProviderConfigurationId = bgpc.GameProviderConfigurationId,
                PlayerCode = playerCode,
                PlatformType = platformType
            };
        }

        private async Task<EvolutionGetPlayerInfo> CheckingUserIdAsync(Guid gameProviderId, string playerCode)
        {
            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            return new EvolutionGetPlayerInfo()
            {
                GameProviderConfigurationId = bgpc.GameProviderConfigurationId,
                PlayerCode = playerCode,
                PlatformType = PlatformType.Desktop
            };
        }
    }
}