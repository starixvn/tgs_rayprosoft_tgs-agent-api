using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.BigGaming.Services;
using TGS.Core.DataServices.Tgs.Models;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.BigGaming.Commons;
using TGS.Shared.Game.Models.BigGaming.Incoming;
using static TGS.Core.Models.Consts;
using GameProvider = TGS.Core.Models.Consts.GameProvider;

namespace TGS.Apis.TgsApi.GameProviders.BigGaming.Middlewares
{
    public sealed class AuthorizeBigGamingMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IHashProvider _hashProvider;
        private readonly IBigGamingService _service;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        public AuthorizeBigGamingMiddleware(
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IHashProvider hashProvider,
            IBigGamingService service,
            IJsonSerializationProvider jsonSerializationProvider)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
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
                "open.operator.user.transfer" => await CheckingSignTransferAsync(_jsonSerializationProvider.DeserializeFromString<BigGamingTransferRequest>(_jsonSerializationProvider.SerializeToString(request.Params))).ConfigureAwait(false),
                "open.operator.ping" => new(true, default, null, default),
                _ => new(false, default, null, default)
            };

            var gameProviderId = await _service.GetGameProviderIdAsync();
            context.Items.Add(ContextItems.GameProviderId, gameProviderId);

            if (checkingSign.Success && !string.IsNullOrEmpty(checkingSign.PlayerCode))
            {
                var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(checkingSign.BrandId, gameProviderId);
                context.Items.Add(ContextItems.PlayerCode, checkingSign.PlayerCode);
                context.Items.Add(ContextItems.PlatformType, checkingSign.PlatformType);
                context.Items.Add(ContextItems.BrandId, checkingSign.BrandId);
                context.Items.Add(ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            }
            await next(context);
        }

        private async Task<(bool Success, Guid BrandId, string PlayerCode, PlatformType PlatformType)> CheckingSignBalanceAsync(BigGamingBalanceRequest request)
        {
            var player = await _service.GetCachedPlayerAsync(request.LoginId);
            if (player == null)
            {
                throw new PlayerNotFoundException($"{request.LoginId }");
            }

            var brandMerchant = await _service.GetBrandMerchantAsync(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            _service.CheckingSign(request.Sign, _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{player.PlayerCode}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5));

            var loginRecord = await _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(request.LoginId).ConfigureAwait(false);

            return new(true, player.BrandId, player.PlayerCode, loginRecord.PlatformType);
        }

        private async Task<(bool Success, Guid BrandId, string PlayerCode, PlatformType PlatformType)> CheckingSignBetAsync(BigGamingBetRequest request)
        {
            var player = await _service.GetCachedPlayerAsync(request.LoginId);
            if (player == null)
            {
                throw new PlayerNotFoundException($"{request.LoginId }");
            }

            var brandMerchant = await _service.GetBrandMerchantAsync(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            _service.CheckingSign(request.Sign, _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{player.PlayerCode}{request.Amount}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5));


            var loginRecord = await _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(request.LoginId).ConfigureAwait(false);

            return new(true, player.BrandId, player.PlayerCode, loginRecord.PlatformType);
        }

        private async Task<(bool Success, Guid BrandId, string PlayerCode, PlatformType PlatformType)> CheckingSignSettleAsync(BigGamingSettleRequest request)
        {
            var player = await _service.GetPlayerByCodeAsync(request.LoginId);
            if (player == null)
            {
                throw new PlayerNotFoundException($"{request.LoginId }");
            }

            var brandMerchant = await _service.GetBrandMerchantAsync(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            _service.CheckingSign(request.Sign, _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{player.PlayerCode}{request.Amount}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5));

            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(player.PlayerCode);

            var loginRecord = await _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(player.PlayerCode);

            return new(true, brandId, player.PlayerCode, loginRecord.PlatformType);
        }

        private async Task<(bool Success, Guid BrandId, string PlayerCode, PlatformType PlatformType)> CheckingSignTransferAsync(BigGamingTransferRequest request)
        {
            Guid brandId;
            string playerCode;
            string defaultWalletCurrencyCode;

            //TGS->GP
            if (request.Amount > 0)
            {
                var player = await _service.GetCachedPlayerAsync(request.LoginId);
                if (player == null)
                {
                    throw new PlayerNotFoundException($"{ request.LoginId }");
                }


                brandId = player.BrandId;
                playerCode = player.PlayerCode;
                defaultWalletCurrencyCode = player.DefaultWalletCurrencyCode;

            }
            else
            {
                var player = await _service.GetPlayerByCodeAsync(request.LoginId);
                if (player == null)
                {
                    throw new PlayerNotFoundException($"{ request.LoginId}");
                }
                brandId = player.BrandId;
                playerCode = player.PlayerCode;
                defaultWalletCurrencyCode = player.DefaultWalletCurrencyCode;
            }

            var brandMerchant = await _service.GetBrandMerchantAsync(defaultWalletCurrencyCode).ConfigureAwait(false);

            _service.CheckingSign(request.Sign, _hashProvider.Hash($"{request.Random }{request.Sn}{playerCode}{request.Amount}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5));

            var loginRecord = await _playerOperations.GetPlayerLatestEnabledLoginRecordAsync(playerCode);

            return new(true, brandId, playerCode, loginRecord.PlatformType);
        }

        private async Task<(bool Success, Guid BrandId, string PlayerCode, PlatformType PlatformType)> CheckingSignOrderStatusAsync(BigGamingOrderStatusRequest request)
        {
            var loginId = request.OrderMap.Last().LoginId;

            var player = await _service.GetPlayerByCodeAsync(loginId);
            if (player == null)
            {
                throw new MissingTokenException(request.Sign);
            }

            var brandMerchant = await _service.GetBrandMerchantAsync(player.DefaultWalletCurrencyCode).ConfigureAwait(false);

            _service.CheckingSign(request.Sign, _hashProvider.Hash($"{request.Random }{brandMerchant.SecretCode}{brandMerchant.SecretKey}", HashAlgorithmKind.Md5));


            return new(true, player.BrandId, player.PlayerCode, PlatformType.Desktop);
        }
    }
}
