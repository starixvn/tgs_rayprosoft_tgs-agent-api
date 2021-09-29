using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Security.Enums;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.KY;
using TGS.Shared.Game.ApiClients.MultiWallet.Defines;
using TGS.Shared.Game.Models.KY.Commons;
using TGS.Shared.Game.Models.KY.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.KY.Services
{
    public record KYGameInitializer : IGameInitializer
    {
        private readonly string _gameProviderCode = Consts.GameProvider.KY;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IKYApiClient _client;
        private readonly IPlayerOperations _playerOperations;
        private readonly ISettingOperations _settingOperations;
        private readonly IHashProvider _hashProvider;
        private readonly IRequestAnalyzer _requestAnalyzer;
        private readonly IGameProviderOperations _gameProviderOperations;


        public KYGameInitializer(
            IHttpContextAccessor httpContextAccessor,
            IKYApiClient client,
            IHashProvider hashProvider,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IRequestAnalyzer requestAnalyzer,
            IGameProviderOperations gameProviderOperations
            )
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _requestAnalyzer = requestAnalyzer ?? throw new ArgumentNullException(nameof(requestAnalyzer));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {

            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);

            var configuration =
                await _settingOperations.GetValueFromJsonByKeyAsync<KYConfiguration>(SettingConsts
                    .GameProviderConfigurations.KY);

            var brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals(player.BrandCode) && x.Status).FirstOrDefault();
            if (brandMerchant == null)
            {
                brandMerchant = configuration.BrandMerchants.Where(x => x.BrandCode.IgnoreCaseEquals("Default") && x.Status).FirstOrDefault();
            }
            var agentId = brandMerchant.AgentMappers.Where(x => x.CurrencyCode.IgnoreCaseEquals(player.DefaultWalletCurrencyCode) && x.Status).FirstOrDefault()?.AgentId;

            var externalID = 0;
            if (request.GameId > 0)
            {
                var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(_gameProviderCode).ConfigureAwait(false);
                var cachedGame = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId).ConfigureAwait(false);
                if (cachedGame != null)
                {
                    if (cachedGame.Type != Core.Models.Enums.GameType.CardGameLobby)
                    {
                        externalID = Convert.ToInt32(cachedGame.ExternalId);
                    }
                }
            }

            var dateTimeOffset = DateTimeOffset.UtcNow.AddHours(+8);
            var orderId = $"{agentId}{dateTimeOffset.ToString("yyyyMMddHHmmssfff")}{player.PlayerCode}";
            var ip = _requestAnalyzer.GetIp(_httpContextAccessor.HttpContext);

            var data = new KYGameUrlRequest()
            {
                PlayerCode = player.PlayerCode,
                OrderId = orderId,
                Amount = 0m,
                IP = ip,
                BrandsCode = player.BrandCode,
                ExternalID = externalID
            };

            var response = await _client.GetGameUrlAsync(brandMerchant, agentId, dateTimeOffset, data).ConfigureAwait(false);

            if (response.Data.Code != 0)
            {
                throw new GameProviderApiErrorException(response.Message);
            }

            return new() { Url = response.Data.Url };
        }
    }
}
