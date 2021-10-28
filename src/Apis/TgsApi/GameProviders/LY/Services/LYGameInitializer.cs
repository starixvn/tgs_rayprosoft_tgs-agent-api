using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.LY;
using TGS.Shared.Game.Models.LY.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.LY.Services
{
    public record LYGameInitializer : IGameInitializer
    {
        private readonly ILYApiClient _client;
        private readonly string _gameProviderCode = Consts.GameProvider.LY;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPlayerOperations _playerOperations;
        private readonly IRequestAnalyzer _requestAnalyzer;
        private readonly ISettingOperations _settingOperations;
        private readonly ILYService _service;

        public LYGameInitializer(
            IHttpContextAccessor httpContextAccessor,
            ILYApiClient client,
            ISettingOperations settingOperations,
            IPlayerOperations playerOperations,
            IRequestAnalyzer requestAnalyzer,
            IGameProviderOperations gameProviderOperations,
            ILYService service
        )
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _settingOperations = settingOperations ?? throw new ArgumentNullException(nameof(settingOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _requestAnalyzer = requestAnalyzer ?? throw new ArgumentNullException(nameof(requestAnalyzer));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        async ValueTask<GetGameUrlResponse> IGameInitializer.GetGameUrlAsync(GetGameUrlRequest request)
        {
            var player = await _playerOperations.GetCachedPlayerAsync(request.Token.PlayerCode);

            var brandMerchant = await _service.GetBrandMerchantAsync(player.BrandCode).ConfigureAwait(false);

            if (brandMerchant == null)
            {
                throw new GameProviderApiErrorException("The brand does not exist");
            }

            var agentId = brandMerchant.AgentMappers
                .Where(x => x.CurrencyCode.IgnoreCaseEquals(player.DefaultWalletCurrencyCode) && x.Status)
                .FirstOrDefault()?.AgentId;

            var externalID = 0;

            if (request.GameId > 0)
            {
                var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(_gameProviderCode)
                    .ConfigureAwait(false);

                var cachedGame = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId)
                    .ConfigureAwait(false);

                if (cachedGame != null)
                {
                    if (cachedGame.Type != GameType.CardGameLobby)
                    {
                        externalID = Convert.ToInt32(cachedGame.ExternalId);
                    }
                }
            }

            var dateTimeOffset = _service.GetDateTimeOffsetTimeZone8();
            var orderId = $"{agentId}{dateTimeOffset.ToString("yyyyMMddHHmmssfff")}{player.PlayerCode}";
            var ip = _requestAnalyzer.GetIp(_httpContextAccessor.HttpContext);

            var data = new LYGameUrlRequest
            {
                PlayerCode = player.PlayerCode,
                OrderId = orderId,
                Amount = 0m,
                IP = ip,
                BrandsCode = player.BrandCode,
                ExternalID = externalID
            };

            var response = await _client.GetGameUrlAsync(brandMerchant.ApiUrl, brandMerchant.Deskey, brandMerchant.Md5key, agentId, dateTimeOffset, data)
                .ConfigureAwait(false);

            if (response.Data.Code != 0)
            {
                throw new GameProviderApiErrorException(response.Message);
            }

            return new()
            {
                Url = response.Data.Url
            };
        }
    }
}
