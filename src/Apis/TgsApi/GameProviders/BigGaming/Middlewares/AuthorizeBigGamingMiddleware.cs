using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.BigGaming.Common;
using TGS.Shared.Game.Models.BigGaming.Incoming;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Middlewares
{
    public sealed class AuthorizeBigGamingMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IHashProvider _hashProvider;
        private readonly string[] _method;
        public AuthorizeBigGamingMiddleware(
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations,
            IHashProvider hashProvider)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _method = new string[] {
                "open.operator.user.balance",
                "open.operator.order.transfer",
                "open.operator.calc.transfer",
                "open.operator.user.transfer",
                "open.operator.order.status"
            };
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var configuration =
             await _settingOperations.GetValueFromJsonByKeyAsync<BigGamingConfiguration>(SettingConsts
                 .GameProviderConfigurations.BigGaming);

            var value = await context.Request.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<BGBaseRequest>(value);

            var checkingSign = response.Method.ToLower() switch
            {
                "open.operator.user.balance" => await CheckingSignBalanceAsync(configuration, JsonSerializer.Deserialize<BGBalanceRequest>(JsonSerializer.Serialize(response.Params))).ConfigureAwait(false),
                "open.operator.order.transfer" => await CheckingSignBetAsync(configuration, JsonSerializer.Deserialize<BGBetRequest>(JsonSerializer.Serialize(response.Params))).ConfigureAwait(false),
                "open.operator.calc.transfer" => await CheckingSignSettleAsync(configuration, JsonSerializer.Deserialize<BGSettleRequest>(JsonSerializer.Serialize(response.Params))).ConfigureAwait(false),
                "open.operator.order.status" => CheckingSignOrderStatusAsync(configuration, JsonSerializer.Deserialize<BGOrderStatusRequest>(JsonSerializer.Serialize(response.Params))),
                _ => new(false, null)
            };

            if (!checkingSign.Success)
            {
                throw new MissingTokenException("empty token");
            }

            if (checkingSign.Success && checkingSign.Player != null)
            {
                var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(checkingSign.Player.PlayerCode);
                var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProvider.BigGaming);
                var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
                var loginRecord = await _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(checkingSign.Player.PlayerCode);
                context.Items.Add(ContextItems.PlayerCode, checkingSign.Player.PlayerCode);
                context.Items.Add(ContextItems.PlatformType, loginRecord.PlatformType);
                context.Items.Add(ContextItems.BrandId, brandId);
                context.Items.Add(ContextItems.GameProviderId, gameProviderId);
                context.Items.Add(ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            }
            await next(context);
        }

        private async Task<(bool Success, CachedPlayer Player)> CheckingSignBalanceAsync(BigGamingConfiguration configuration, BGBalanceRequest request)
        {

            if (_hashProvider.Hash($"{request.Random }{configuration.SecretCode}{request.LoginId}{configuration.SecretKey}", HashAlgorithmKind.Md5) != request.Sign)
            {
                throw new MissingTokenException("empty token");
            }
            var player = await GetPlayerAsync(request.LoginId);
            return new(player != null, player);
        }

        private async Task<(bool Success, CachedPlayer Player)> CheckingSignBetAsync(BigGamingConfiguration configuration, BGBetRequest request)
        {
            if (_hashProvider.Hash($"{request.Random }{configuration.SecretCode}{request.LoginId}{request.Amount}{configuration.SecretKey}", HashAlgorithmKind.Md5) != request.Sign)
            {
                throw new MissingTokenException("empty token");
            }
            var player = await GetPlayerAsync(request.LoginId);
            return new(player != null, player);
        }
        private async Task<(bool Success, CachedPlayer Player)> CheckingSignSettleAsync(BigGamingConfiguration configuration, BGSettleRequest request)
        {
            if (_hashProvider.Hash($"{request.Random }{configuration.SecretCode}{request.LoginId}{request.Amount}{configuration.SecretKey}", HashAlgorithmKind.Md5) != request.Sign)
            {
                throw new MissingTokenException("empty token");
            }

            var player = await GetPlayerAsync(request.LoginId);
            return new(player != null, player);
        }

        private (bool Success, CachedPlayer Player) CheckingSignOrderStatusAsync(BigGamingConfiguration configuration, BGOrderStatusRequest request)
        {
            if (_hashProvider.Hash($"{request.Random }{configuration.SecretCode}{configuration.SecretKey}", HashAlgorithmKind.Md5) != request.Sign)
            {
                throw new MissingTokenException("empty token");
            }

            return new(true, null);
        }

        private async Task<CachedPlayer> GetPlayerAsync(string playerCode)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(playerCode).ConfigureAwait(false);
            if (player == null)
            {
                throw new PlayerLoginExpiredException();
            }
            return player;
        }
    }
}
