// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.IMESports.Enums;
using TGS.Shared.Game.Models.IMESports.Incoming;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.IMESports.Middlewares
{
    public sealed class AuthorizeIMESportsMiddleware : IMiddleware
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ISettingOperations _settingOperations;

        public AuthorizeIMESportsMiddleware(
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations)
        {
            _settingOperations = settingOperations;
            _gameProviderOperations = gameProviderOperations;
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var configuration = await _settingOperations
                .GetValueFromJsonByKeyAsync<IMESportsConfiguration>(SettingConsts.GameProviderConfigurations.IMESports);

            var request = await context.Request.ReadAsObjectFromJsonAsync<Base>();

            if (!Enum.TryParse(typeof(ProductWalletType), request.ProductWallet, out var productWalletType) &&
                (int) productWalletType != configuration.ProductWallet)
            {
                throw new ArgumentException($"Invalid ProductWallet: {request.ProductWallet}");
            }

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProvider.IMESports);

            context.Items.Add(ContextItems.GameProviderId, gameProviderId);

            await next(context);
        }
    }
}