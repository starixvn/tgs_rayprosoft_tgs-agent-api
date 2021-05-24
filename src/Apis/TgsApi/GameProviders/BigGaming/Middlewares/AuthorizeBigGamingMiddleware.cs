using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.BigGaming.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
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
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        public AuthorizeBigGamingMiddleware(
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            ISettingOperations settingOperations,
            IHashProvider hashProvider,
            IBigGamingServices bigGamingServices,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _bigGamingServices = bigGamingServices ?? throw new ArgumentNullException(nameof(bigGamingServices));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var configuration =
             await _settingOperations.GetValueFromJsonByKeyAsync<BigGamingConfiguration>(SettingConsts
                 .GameProviderConfigurations.BigGaming);

            var value = await context.Request.ReadAsStringAsync();
            var request = JsonSerializer.Deserialize<BigGamingRequest>(value);

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

            var checkingSign = request.Method.ToLower() switch
            {
                "open.operator.user.balance" => await CheckingSignBalanceAsync(_jsonSerializationProvider.DeserializeFromString<BigGamingBalanceRequest>(_jsonSerializationProvider.SerializeToString(request.Params))).ConfigureAwait(false),
                "open.operator.order.transfer" => await CheckingSignBetAsync(_jsonSerializationProvider.DeserializeFromString<BigGamingBetRequest>(_jsonSerializationProvider.SerializeToString(request.Params))).ConfigureAwait(false),
                "open.operator.calc.transfer" => await CheckingSignSettleAsync(_jsonSerializationProvider.DeserializeFromString<BigGamingSettleRequest>(_jsonSerializationProvider.SerializeToString(request.Params))).ConfigureAwait(false),
                "open.operator.order.status" => await CheckingSignOrderStatusAsync(_jsonSerializationProvider.DeserializeFromString<BigGamingOrderStatusRequest>(_jsonSerializationProvider.SerializeToString(request.Params))).ConfigureAwait(false),
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
                context.Items.Add(ContextItems.PlayerCode, loginRecord.PlayerCode);
                context.Items.Add(ContextItems.PlatformType, loginRecord.PlatformType);
                context.Items.Add(ContextItems.BrandId, brandId);
                context.Items.Add(ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            }
            await next(context);
        }

        private async Task<(bool Success, CachedPlayer Player)> CheckingSignBalanceAsync(BigGamingBalanceRequest request)
        {
            var player = await GetPlayerAsync(request.LoginId);
            if (player == null)
            {
                throw new PlayerNotFoundException($"{request.LoginId }");
            }

            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            CheckingSign(request.Sign, _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{player.PlayerCode}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5));

            return new(true, player);
        }

        private async Task<(bool Success, CachedPlayer Player)> CheckingSignBetAsync(BigGamingBetRequest request)
        {
            var player = await GetPlayerAsync(request.LoginId);
            if (player == null)
            {
                throw new PlayerNotFoundException($"{request.LoginId }");
            }

            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            CheckingSign(request.Sign, _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{player.PlayerCode}{request.Amount}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5));

            if (decimal.Parse(request.Amount) != request.Orders.Sum(x => x.Amount))
            {
                throw new InvalidAmountException($"The total amount ({decimal.Parse(request.Amount)}) does not match the order amount ({request.Orders.Sum(x => x.Amount)})");
            }

            return new(true, player);
        }
        private async Task<(bool Success, CachedPlayer Player)> CheckingSignSettleAsync(BigGamingSettleRequest request)
        {
            var player = await GetPlayerAsync(request.LoginId);
            if (player == null)
            {
                throw new PlayerNotFoundException($"{request.LoginId }");
            }

            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            CheckingSign(request.Sign, _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{player.PlayerCode}{request.Amount}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5));

            if (decimal.Parse(request.Amount) != request.Orders.Sum(x => x.Amount))
            {
                throw new InvalidAmountException($"The total amount ({decimal.Parse(request.Amount)}) does not match the order amount ({request.Orders.Sum(x => x.Amount)})");
            }

            return new(true, player);
        }

        private async Task<(bool Success, CachedPlayer Player)> CheckingSignOrderStatusAsync(BigGamingOrderStatusRequest request)
        {
            var loginId = request.OrderMap.Last().LoginId;

            var player = await GetPlayerAsync(loginId);
            if (player == null)
            {
                throw new MissingTokenException(request.Sign);
            }

            var brandMerchant = await _bigGamingServices.GetBrandMerchant(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            CheckingSign(request.Sign, _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5));

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

        private void CheckingSign(string sourceSign, string Sign)
        {
            if (sourceSign.IgnoreCaseNotEquals(Sign))
            {
                throw new MissingTokenException($"Authentication encryption failed({sourceSign})");
            }
        }
    }
}
