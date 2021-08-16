using System.Security.Authentication;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameProviders.WMCasino.Services;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.WMCasino.Middlewares
{
    public class AuthorizeWMCasinoMiddleware : IMiddleware
    {
        private readonly IWMCasinoAgentMapper _agentMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;

        public AuthorizeWMCasinoMiddleware(
            ISettingOperations settingOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandOperations brandOperations,
            IPlayerOperations playerOperations,
            IWMCasinoAgentMapper agentMapper)
        {
            _settingOperations = settingOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandOperations = brandOperations;
            _playerOperations = playerOperations;
            _agentMapper = agentMapper;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var data = await context.Request.ReadAsStringAsync();
            var document = JsonDocument.Parse(data);

            if (!document.RootElement.TryGetProperty("signature", out var element))
            {
                throw new InvalidCredentialException("Missing signature");
            }

            var signature = element.GetString();
            if (signature.IsNullOrEmpty())
            {
                throw new InvalidCredentialException("Missing signature");
            }

            if (!document.RootElement.TryGetProperty("user", out element))
            {
                await next(context);
                return;
            }

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.AllBet);
            context.Items.Add(Consts.ContextItems.GameProviderId, gameProviderId);

            var user = element.GetString();
            if (user.IsNullOrEmpty())
            {
                await next(context);
                return;
            }

            var setting =
                await _settingOperations.GetValueFromJsonByKeyAsync<WMCasinoConfiguration>(SettingConsts
                    .GameProviderConfigurations.WMCasino);
            var agent = _agentMapper.GetAgent(setting.BrandMerchants, signature);

            if (agent == null)
            {
                throw new InvalidCredentialException("No agent is found.");
            }

            var playerCode = user;
            context.Items.Add(Consts.ContextItems.PlayerCode, playerCode);

            if (!context.Request.Path.StartsWithSegments("/api/wm/balance"))
            {
                var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
                var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId,
                    gameProviderId);
                var gameProviderConfigurationId = bgpc.GameProviderConfigurationId;

                context.Items.Add(Consts.ContextItems.GameProviderConfigurationId, gameProviderConfigurationId);
            }

            await next(context);
        }
    }
}