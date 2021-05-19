using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.BigGaming.Services;
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
        private readonly IBigGamingServices _bigGamingServices;
        public AuthorizeBigGamingMiddleware(
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations,
            IHashProvider hashProvider,
            IBigGamingServices bigGamingServices)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _bigGamingServices = bigGamingServices ?? throw new ArgumentNullException(nameof(bigGamingServices));
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var configuration =
             await _settingOperations.GetValueFromJsonByKeyAsync<BigGamingConfiguration>(SettingConsts
                 .GameProviderConfigurations.BigGaming);

            var value = await context.Request.ReadAsStringAsync();
            var response = JsonSerializer.Deserialize<BigGamingBaseRequest>(value);

            /*
            if (context.Request.Path.Value.Contains("/api/bg/entrance", StringComparison.InvariantCultureIgnoreCase))
            {
                context.Request.Path = response.Method.ToLower() switch
                {
                    "open.operator.user.balance" => "/api/bg/balance",
                    "open.operator.order.transfer" => "/api/bg/bet",
                    "open.operator.calc.transfer" => "/api/bg/settle",
                    "open.operator.order.status" => "/api/bg/orderstatus",
                    "open.operator.ping" => "/api/bg/ping",
                    _ => context.Request.Path.Value
                };

            }
            */

            var checkingSign = response.Method.ToLower() switch
            {
                "open.operator.user.balance" => await CheckingSignBalanceAsync(configuration, JsonSerializer.Deserialize<BigGamingBalanceRequest>(JsonSerializer.Serialize(response.Params))).ConfigureAwait(false),
                "open.operator.order.transfer" => await CheckingSignBetAsync(configuration, JsonSerializer.Deserialize<BigGamingBetRequest>(JsonSerializer.Serialize(response.Params))).ConfigureAwait(false),
                "open.operator.calc.transfer" => await CheckingSignSettleAsync(configuration, JsonSerializer.Deserialize<BigGamingSettleRequest>(JsonSerializer.Serialize(response.Params))).ConfigureAwait(false),
                "open.operator.order.status" => await CheckingSignOrderStatusAsync(configuration, JsonSerializer.Deserialize<BigGamingOrderStatusRequest>(JsonSerializer.Serialize(response.Params))).ConfigureAwait(false),
                "open.operator.ping" => new(true, null),
                _ => new(false, null)
            };

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProvider.BigGaming);
            context.Items.Add(ContextItems.GameProviderId, gameProviderId);

            if (checkingSign.Success && checkingSign.Player != null)
            {
                var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(checkingSign.Player.PlayerCode);
                var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
                var loginRecord = await _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(checkingSign.Player.PlayerCode);
                context.Items.Add(ContextItems.PlayerCode, checkingSign.Player.PlayerCode);
                context.Items.Add(ContextItems.PlatformType, loginRecord.PlatformType);
                context.Items.Add(ContextItems.BrandId, brandId);
                context.Items.Add(ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            }
            await next(context);
        }

        private async Task<(bool Success, CachedPlayer Player)> CheckingSignBalanceAsync(BigGamingConfiguration configuration, BigGamingBalanceRequest request)
        {
            var player = await GetPlayerAsync(request.LoginId);
            if (player == null)
            {
                throw new PlayerNotFoundException($"{request.LoginId }");
            }

            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            if (_hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{player.PlayerCode}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5) != request.Sign)
            {
                throw new MissingTokenException("Sign Error");
            }

            return new(player != null, player);
        }

        private async Task<(bool Success, CachedPlayer Player)> CheckingSignBetAsync(BigGamingConfiguration configuration, BigGamingBetRequest request)
        {
            var player = await GetPlayerAsync(request.LoginId);
            if (player == null)
            {
                throw new PlayerNotFoundException($"{request.LoginId }");
            }

            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            if (_hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{player.PlayerCode}{request.Amount}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5) != request.Sign)
            {
                throw new MissingTokenException("Sign Error");
            }

            return new(player != null, player);
        }
        private async Task<(bool Success, CachedPlayer Player)> CheckingSignSettleAsync(BigGamingConfiguration configuration, BigGamingSettleRequest request)
        {
            var player = await GetPlayerAsync(request.LoginId);
            if (player == null)
            {
                throw new PlayerNotFoundException($"{request.LoginId }");
            }

            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            if (_hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{player.PlayerCode}{request.Amount}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5) != request.Sign)
            {
                throw new MissingTokenException("Sign Error");
            }

            return new(player != null, player);
        }

        private async Task<(bool Success, CachedPlayer Player)> CheckingSignOrderStatusAsync(BigGamingConfiguration configuration, BigGamingOrderStatusRequest request)
        {
            var loginId = request.OrderMap.Last().LoginId;

            var player = await GetPlayerAsync(loginId);
            if (player == null)
            {
                throw new MissingTokenException("Sign Error");
            }

            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            if (_hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5) != request.Sign)
            {
                throw new MissingTokenException("Sign Error");
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
