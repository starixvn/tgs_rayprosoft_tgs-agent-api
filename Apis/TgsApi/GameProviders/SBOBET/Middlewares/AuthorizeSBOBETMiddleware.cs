// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.GameModels.SBOBET.Incoming;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Middlewares
{
    public sealed class AuthorizeSBOBETMiddleware : IMiddleware
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IBrandOperations _brandOperations;
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
            var externalGameId = request.ExternalGameId;

            var brandId = await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(GameProvider.SBOBET);
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);

            PlatformType platformType;
            var games = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId, externalGameId);
            if (games.Length == 1)
            {
                platformType = games[0].PlatformType;
            }
            else
            {
                var loginRecords = await _playerOperations.GetPlayerEnabledLoginRecordsAsync(playerCode);
                platformType = (from rec in loginRecords
                                join g in games on rec.PlatformType equals g.PlatformType
                                orderby rec.ExpiredOn descending
                                select rec.PlatformType)
                    .DefaultIfEmpty(PlatformType.Desktop)
                    .FirstOrDefault();
            }

            context.Items.Add(ContextItems.BrandId, brandId);
            context.Items.Add(ContextItems.GameProviderId, gameProviderId);
            context.Items.Add(ContextItems.GameProviderConfigurationId, bgpc.GameProviderConfigurationId);
            context.Items.Add(ContextItems.PlatformType, platformType);

            await next(context);
        }
    }
}