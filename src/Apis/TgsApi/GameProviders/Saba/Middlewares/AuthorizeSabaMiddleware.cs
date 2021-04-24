using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.Saba.Attributes;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.Saba.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.Saba.Middlewares
{
    public sealed class AuthorizeSabaMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IRedisCacheManager _cacheManger;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IJsonSerializationProvider _json;
        private readonly IPlayerOperations _playerOperations;

        private readonly IReadOnlyDictionary<Type, Func<string, ValueTask<(string, string, PlatformType)>>> _resolver;
        private readonly ISettingOperations _settingOperations;

        public AuthorizeSabaMiddleware(
            IJsonSerializationProvider json,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations,
            IRedisCacheManager cacheManger)
        {
            _json = json;
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _settingOperations = settingOperations;
            _cacheManger = cacheManger;

            _resolver = new Dictionary<Type, Func<string, ValueTask<(string, string, PlatformType)>>>
            {
                {typeof(GetBalanceRequest), ResolveGetBalanceRequestAsync},
                {typeof(PlaceBetRequest), ResolvePlaceBetRequestAsync},
                {typeof(PlaceBetParlayRequest), ResolvePlaceBetParlayRequestAsync},
                {typeof(ConfirmBetRequest), ResolveConfirmBetRequestAsync},
                {typeof(ConfirmBetParlayRequest), ResolveConfirmBetParlayRequestAsync},
                {typeof(CancelBetRequest), ResolveCancelBetRequestAsync}
            };
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var value = await context.Request.ReadAsStringAsync();

            var attribute = context.GetEndpoint()?.Metadata?.GetMetadata<ResolveRequestAttribute>();

            if (attribute == null)
            {
                throw new InvalidOperationException("Missing RequirePlayerTokenAttribute attribute settings");
            }

            if (!_resolver.TryGetValue(attribute.Type, out var execute))
            {
                throw new InvalidOperationException($"Unexpected resolver type: {attribute.Type.Name}");
            }

            var (secretKey, playerCode, platformType) = await execute(value);

            var configuration = await _settingOperations
                .GetValueFromJsonByKeyAsync<SabaConfiguration>(SettingConsts.GameProviderConfigurations
                    .Saba);

            if (configuration.SecretKey.NotEquals(secretKey))
            {
                throw new InvalidCredentialsException("Invalid key");
            }

            var gameProviderId =
                await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.Saba);
            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);
            context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);
            context.Items.Add(Consts.ContextItems.PlatformType, platformType);

            await next(context);
        }

        private ValueTask<(string SecretKey, string PlayerCode, PlatformType PlatformType)>
            ResolveGetBalanceRequestAsync(string value)
        {
            var r = _json.DeserializeFromString<GetBalanceRequest>(value);
            return ValueTask.FromResult((r.Key, r.Body.PlayerCode, PlatformType.Desktop));
        }

        private async ValueTask<(string SecretKey, string PlayerCode, PlatformType PlatformType)>
            ResolvePlaceBetRequestAsync(string value)
        {
            var r = _json.DeserializeFromString<PlaceBetRequest>(value);
            var loginRecord = await DecryptTokenAsync(r.Body.Token);
            var playerCode = loginRecord.PlayerCode;

            await CachedLoginRecordAsync(playerCode, loginRecord);

            return (r.Key, playerCode, loginRecord.PlatformType);
        }

        private async ValueTask<(string, string, PlatformType)> ResolvePlaceBetParlayRequestAsync(string value)
        {
            var r = _json.DeserializeFromString<PlaceBetParlayRequest>(value);
            var loginRecord = await DecryptTokenAsync(r.Body.Token);
            var playerCode = loginRecord.PlayerCode;

            await CachedLoginRecordAsync(playerCode, loginRecord);

            return (r.Key, playerCode, loginRecord.PlatformType);
        }

        private async ValueTask<(string, string, PlatformType)> ResolveConfirmBetRequestAsync(string value)
        {
            var r = _json.DeserializeFromString<ConfirmBetRequest>(value);
            var playerCode = r.Body.PlayerCode;

            var platformType = await GetPlatformTypeFromCachedLoginRecordAsync(playerCode);
            return (r.Key, playerCode, platformType);
        }

        private async ValueTask<(string, string, PlatformType)> ResolveCancelBetRequestAsync(string value)
        {
            var r = _json.DeserializeFromString<CancelBetRequest>(value);
            var playerCode = r.Body.PlayerCode;

            var platformType = await GetPlatformTypeFromCachedLoginRecordAsync(playerCode);
            return (r.Key, playerCode, platformType);
        }

        private async ValueTask<(string, string, PlatformType)> ResolveConfirmBetParlayRequestAsync(string value)
        {
            var r = _json.DeserializeFromString<ConfirmBetParlayRequest>(value);
            var playerCode = r.Body.PlayerCode;

            var platformType = await GetPlatformTypeFromCachedLoginRecordAsync(playerCode);
            return (r.Key, playerCode, platformType);
        }


        private async Task<CachedPlayerLoginRecord> DecryptTokenAsync(string token)
        {
            if (token.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }

            var loginRecordId = Guid.Parse(token);
            var loginRecord = await _playerOperations.GetPlayerLoginRecordAsync(loginRecordId);

            if (loginRecord == null)
            {
                throw new PlayerLoginExpiredException();
            }

            return loginRecord;
        }

        private async ValueTask<PlatformType> GetPlatformTypeFromCachedLoginRecordAsync(string playerCode)
        {
            var loginRecord = await _cacheManger.SafeGetAsync<CachedPlayerLoginRecord>(SabaConsts.RedisKey.Token,
                playerCode, () => null);

            return loginRecord?.PlatformType ?? PlatformType.Desktop;
        }

        private async ValueTask CachedLoginRecordAsync(string playerCode, CachedPlayerLoginRecord loginRecord)
        {
            var expiresOn = loginRecord.ExpiredOn - DateTimeOffset.Now;
            await _cacheManger.SafeCacheAsync(SabaConsts.RedisKey.Token, playerCode, expiresOn, loginRecord);
        }
    }
}