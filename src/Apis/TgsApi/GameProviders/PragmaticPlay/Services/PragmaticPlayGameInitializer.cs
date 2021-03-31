using System.Threading.Tasks;
using System.Web;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Services
{
    public sealed class PragmaticPlayGameInitializer : IGameInitializer
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly ITokenProvider _tokenProvider;

        public PragmaticPlayGameInitializer(
            ITokenProvider tokenProvider,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations)
        {
            _tokenProvider = tokenProvider;
            _settingOperations = settingOperations;
            _playerOperations = playerOperations;
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _brandGameProviderOperations = brandGameProviderOperations;
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var token = request.Token;
            var loginTokenId = token.LoginRecordId;
            var encryptedToken = await _tokenProvider.EncryptAsync(loginTokenId);

            var player = await _playerOperations.GetCachedPlayerAsync(token.PlayerCode);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);

            var brandId = player.BrandId;
            var bgpc = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId);
            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(gameProviderId, brandId,
                request.Language);

            var gpc = await _gameProviderOperations.GetCachedGameProviderConfigurationAsync(gameProviderId,
                bgpc.GameProviderConfigurationId);

            var game = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<PragmaticPlayConfiguration>(SettingConsts
                    .GameProviderConfigurations.Habanero);

            var isMobile = token.PlatformType == PlatformType.Mobile;

            var queryString = configuration.GameUrlQueryStringTemplate
                .Replace("{TOKEN}", encryptedToken)
                .Replace("{CULTURE}", cultureCode)
                .Replace("{EXTERNALGAMEID}", game.ExternalId)
                .Replace("{TECHNOLOGY}", "H5")
                .Replace("{PLATFORM}", isMobile ? "MOBILE" : "WEB");

            var domain = isMobile ? gpc.MobileEndpoint : gpc.DesktopEndpoint;

            var url = domain.AppendQueryString(game.EndpointPath)
                .Replace("{KEY}", HttpUtility.UrlEncode(queryString))
                .Replace("{SECURELOGIN}", configuration.SecureLogin);

            return new() {Url = url};
        }
    }
}