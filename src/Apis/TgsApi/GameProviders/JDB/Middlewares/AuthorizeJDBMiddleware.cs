using System;
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
using TGS.Shared.Game.Models.JDB.Enums;
using TGS.Shared.Game.Models.JDB.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.JDB.Middlewares
{
    public sealed class AuthorizeJDBMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IJsonSerializationProvider _json;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;

        public AuthorizeJDBMiddleware(
            IJsonSerializationProvider json,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations,
            ITokenProvider tokenProvider)
        {
            _json = json;
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _settingOperations = settingOperations;
            _tokenProvider = tokenProvider;
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var value = await context.Request.ReadAsStringAsync();
            var request = _json.DeserializeFromString<BaseRequest>(value);

            JDBConsts.RequestTypeMapper.TryGetValue(request.Type, out var type);
            context.Items.Add(Consts.ContextItems.RequestType, type);

            var clientId = request.Authorization?.ClientId;
            var secretKey = request.Authorization?.SecretKey;
            var habaneroBrandId = request.Authorization?.HabaneroBrandId;

            if (clientId.IsNullOrEmpty() || secretKey.IsNullOrEmpty() || habaneroBrandId.IsNullOrEmpty())
            {
                throw new InvalidCredentialsException("Empty username or passkey or brandid");
            }

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<HabaneroConfiguration>(SettingConsts
                    .GameProviderConfigurations.Habanero);

            if (configuration.BrandId.NotEquals(habaneroBrandId))
            {
                throw new InvalidCredentialsException("invalid brandid");
            }

            var ids = await _gameProviderOperations.GetGameProviderIdsByCredentialsAsync(clientId,
                secretKey);
            if (ids == null || Consts.GameProvider.Habanero.NotEquals(ids.Code))
            {
                throw new InvalidCredentialsException("invalid username and passkey");
            }

            var playerCode = string.Empty;
            var platformType = PlatformType.Desktop;

            if (type == typeof(AuthorizePlayerRequest))
            {
                var r = _json.DeserializeFromString<AuthorizePlayerRequest>(value);
                var loginRecord = await DecryptTokenAsync(r.Detail.Token, false);
                playerCode = loginRecord.PlayerCode;
                platformType = loginRecord.PlatformType;
            }
            else if (type == typeof(FundTransferRequest))
            {
                var r = _json.DeserializeFromString<FundTransferRequest>(value);
                var actionType = r.Detail.ActionType;
                var allowExpiredLogin = actionType is not ActionType.PlaceAndSettleBet and not ActionType.PlaceBet;
                var loginRecord = await DecryptTokenAsync(r.Detail.Token, allowExpiredLogin);

                if (allowExpiredLogin && loginRecord == null)
                {
                    playerCode = r.Detail.PlayerCode;
                }
                else
                {
                    playerCode = loginRecord.PlayerCode;
                    platformType = loginRecord.PlatformType;
                }
            }

            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);

            var gameProviderId = ids.Id;
            var gameProviderConfigurationId = ids.ConfigurationId;
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
            if (bgpc == null || bgpc.GameProviderConfigurationId != gameProviderConfigurationId)
            {
                throw new InvalidCredentialsException("invalid username and passkey for brand configuration");
            }

            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);
            context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, gameProviderConfigurationId);
            context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);
            context.Items.Add(Consts.ContextItems.PlatformType, platformType);

            await next(context);
        }

        private async Task<CachedPlayerLoginRecord> DecryptTokenAsync(
            string token, bool allowExpiredLogin)
        {
            if (token.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }

            var loginRecordId = await _tokenProvider.DecryptAsync<Guid>(token);
            var loginRecord = await _playerOperations.GetPlayerLoginRecordAsync(loginRecordId);

            if (!allowExpiredLogin && loginRecord == null)
            {
                throw new PlayerLoginExpiredException();
            }

            return loginRecord;
        }
    }
}