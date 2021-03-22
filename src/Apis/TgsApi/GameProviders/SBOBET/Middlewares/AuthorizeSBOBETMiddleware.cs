// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Shared.Game.Models.SBOBET.Incoming;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Middlewares
{
    public sealed class AuthorizeSBOBETMiddleware : IMiddleware
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public AuthorizeSBOBETMiddleware(
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandOperations brandOperations)
        {
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandOperations = brandOperations;
        }

        async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var configuration = await _settingOperations
                .GetValueFromJsonByKeyAsync<SBOConfiguration>(SettingConsts.GameProviderConfigurations.SBOBET);

            context.Request.EnableBuffering();

            var request = await context.Request.ReadAsObjectFromJsonAsync<BaseRequest>();

            if (request.CompanyKey.IsNullOrEmpty() || request.CompanyKey.NotEquals(configuration.CompanyKey))
            {
                throw new InvalidCredentialsException($"Invalid company key: {request.CompanyKey}");
            }

            var playerCode = request.PlayerCode;
            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProvider.SBOBET);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            context.Items.Add(ContextItems.BrandId, brandId);
            context.Items.Add(ContextItems.GameProviderId, gameProviderId);
            context.Items.Add(ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);

            await next(context);
        }
    }
}