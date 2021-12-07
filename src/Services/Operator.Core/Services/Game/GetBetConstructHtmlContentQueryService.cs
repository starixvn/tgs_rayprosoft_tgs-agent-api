using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Tgs.Core.Infrastructures.Security;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Tgs;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;

namespace TGS.Services.Operator.Services.Game
{
    public class GetBetConstructHtmlContentQueryService : IGetService<GetBetConstructHtmlContentQuery, BetConstructHtmlContentInfo>
    {
        private readonly IBrandOperations _brandOperations;
        private readonly IGameProviderCurrencyCodeMapper _currencyCodeMapper;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ITgsRepository _repository;
        private readonly ITokenProvider _tokenProvider;

        public GetBetConstructHtmlContentQueryService(
            ITokenProvider tokenProvider,
            ITgsRepository repository,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IGameProviderCurrencyCodeMapper currencyCodeMapper,
            IPlayerOperations playerOperations)
        {
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _currencyCodeMapper = currencyCodeMapper ?? throw new ArgumentNullException(nameof(currencyCodeMapper));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        public async ValueTask<BetConstructHtmlContentInfo> GetAsync(GetBetConstructHtmlContentQuery query, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _gameProviderOperations
                .GetGameProviderIdByCodeAsync(Consts.GameProvider.BetConstruct)
                .ConfigureAwait(false);

            var brandGameProviderConfiguration = await _brandOperations
                .GetBrandGameProviderConfigurationAsync(query.BrandId, gameProviderId)
                .ConfigureAwait(false);

            var gameProviderConfiguration = await _gameProviderOperations
                .GetCachedGameProviderConfigurationAsync(gameProviderId, brandGameProviderConfiguration.GameProviderConfigurationId)
                .ConfigureAwait(false);

            var player = await _playerOperations.GetCachedPlayerAsync(query.PlayerCode);

            _ = await _currencyCodeMapper.InternalToExternalAsync(player.DefaultWalletCurrencyCode, gameProviderId);

            var domain = query.PlatformType switch
            {
                PlatformType.Desktop => gameProviderConfiguration.DesktopEndpoint,
                PlatformType.Mobile => gameProviderConfiguration.MobileEndpoint,
                _ => gameProviderConfiguration.DesktopEndpoint
            };

            var betConstructSetting = _repository.Settings
                .AsQueryable()
                .Where(x => x.Key == SettingConsts.GameProviderConfigurations.BetConstruct)
                .Select(x => x.Value)
                .FirstOrDefault()!;

            var userId = JsonDocument.Parse(betConstructSetting).RootElement.GetProperty("PartnerId").GetString();

            var authToken = await _tokenProvider.EncryptAsync(query.PlayerCode);

            var lang = query.CultureName switch
            {
                "zh-CN" => "chi",
                "zh-TW" => "chi",
                "th-TH" => "tha",
                "vi-VN" => "vnm",
                _ => "eng"
            };

            var html =
                "<div id=\"bcsportsbookcontainer\"></div>" +
                $"<script id=\"bcsportsbook\" src=\"{domain}/js/partnerinit.js?containerID=bcsportsbookcontainer&UserId={userId}&AuthToken={authToken}&lang={lang}&sport=1\"></script>";

            return new BetConstructHtmlContentInfo()
            {
                Html = html
            };
        }
    }
}
