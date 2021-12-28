using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.TF.Incoming;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.TF.Middlewares
{
    public sealed class AuthorizeTFMiddleware : IMiddleware
    {
        private static readonly string _Route = "/tf/api";
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public AuthorizeTFMiddleware(
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations)
        {
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var method = context.Request.Method;
            var path = context.Request.Path;

            if (HttpMethods.IsPost(method)
                && path.StartsWithSegments($"{_Route}/token/validate"))
            {
                await next(context);
                return;
            }

            var authorization = context.Request.Headers["Authorization"];

            var playerCode = string.Empty;
            var internalCurrencyCode = string.Empty;
            if (HttpMethods.IsGet(method) && path.StartsWithSegments($"{_Route}/wallet"))
            {
                playerCode = context.Request.Query["loginName"];
            }
            else if (HttpMethods.IsPost(method)
                && (path.StartsWithSegments($"{_Route}/transfer")
                    || path.StartsWithSegments($"{_Route}/rollback")))
            {
                var value = await context.Request.ReadAsStringAsync();
                var transferRequest = JsonSerializer.Deserialize<TransferRequest>(value);
                var ticketDetail = transferRequest.TicketDetail;

                playerCode = transferRequest.LoginName;

                var platformType = ticketDetail.RequestSource switch
                {
                    "desktop-browser" => PlatformType.Desktop,
                    "mobile-browser" => PlatformType.Mobile,
                    "mobile-app" => PlatformType.Mobile,
                    "unknown" => PlatformType.Desktop,
                    _ => PlatformType.Desktop
                };
                context.Items.Add(ContextItems.PlatformType, platformType);

                var gameProviderId = await _gameProviderOperations
                    .GetGameProviderIdByCodeAsync(GameProvider.TFGaming)
                    .ConfigureAwait(false);

                context.Items.Add(ContextItems.GameProviderId, gameProviderId);

                playerCode = transferRequest.LoginName;

                var brandId = await _playerOperations
                    .GetBrandIdByPlayerCodeAsync(playerCode)
                    .ConfigureAwait(false);

                var bgpc = await _brandOperations
                    .GetBrandGameProviderConfigurationAsync(brandId, gameProviderId)
                    .ConfigureAwait(false);

                context.Items.Add(ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
                context.Items.Add(ContextItems.PlayerCode, playerCode);
            }

            var player = await _playerOperations
                .GetPlayerByCodeAsync(playerCode)
                .ConfigureAwait(false);

            internalCurrencyCode = player.DefaultWalletCurrencyCode;

            var configuration = await _settingOperations
                .GetValueFromJsonByKeyAsync<TFConfiguration>(SettingConsts.GameProviderConfigurations.TFGaming)
                .ConfigureAwait(false);

            var brandMerchant = configuration.BrandMerchants.FirstOrDefault(bm => bm.BrandCurrency == internalCurrencyCode);

            if (brandMerchant == null || brandMerchant.PrivateToken != authorization)
            {
                throw new InvalidCredentialsException("");
            }

            await next(context);
        }
    }
}
