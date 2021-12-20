using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Core.Infrastructures.Security;
using TGS.Apis.TgsApi.Models.Interfaces;
using TGS.Core.Models.Exceptions;
using TGS.Services.Operations;
using TGS.Services.PluggableServices.Models;
using TGS.Shared.Game.ApiClients.Jili;
using TGS.Shared.Game.Models.Jili.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Services
{
    public class JiliGameInitializer : IGameInitializer
    {
        private readonly IBrandGameProviderOperations _brandGameProviderOperations;
        private readonly IJiliApiClient _client;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ILogger<JiliGameInitializer> _logger;
        private readonly IJiliMerchantMapper _merchantMapper;
        private readonly IPlayerOperations _playerOperations;
        private readonly IJiliService _service;
        private readonly ITokenProvider _tokenProvider;

        public JiliGameInitializer(
            ILogger<JiliGameInitializer> logger,
            ITokenProvider tokenProvider,
            IPlayerOperations playerOperations,
            IGameProviderOperations gameProviderOperations,
            IBrandGameProviderOperations brandGameProviderOperations,
            IJiliMerchantMapper merchantMapper,
            IJiliService service,
            IJiliApiClient client)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _brandGameProviderOperations = brandGameProviderOperations ?? throw new ArgumentNullException(nameof(brandGameProviderOperations));
            _merchantMapper = merchantMapper ?? throw new ArgumentNullException(nameof(merchantMapper));
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async ValueTask<GetGameUrlResponse> GetGameUrlAsync(GetGameUrlRequest request)
        {
            var tokenData = request.Token;
            var token = $"{tokenData.PlayerCode},{tokenData.LoginRecordId},{tokenData.PlatformType}";
            var encryptedToken = await _tokenProvider.EncryptAsync(token).ConfigureAwait(false);

            var player = await _playerOperations.GetCachedPlayerAsync(tokenData.PlayerCode).ConfigureAwait(false);

            var gameProviderId = await _gameProviderOperations
                .GetGameProviderIdByCodeAsync(request.GameProviderCode)
                .ConfigureAwait(false);

            var cultureCode = await _brandGameProviderOperations.MapCultureAsync(
                gameProviderId,
                player.BrandId,
                request.Language)
                .ConfigureAwait(false);

            var game = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, request.GameId)
                .ConfigureAwait(false);

            var configuration = await _service.GetConfigurationAsync().ConfigureAwait(false);

            var merchant = _merchantMapper.GetMerchant(configuration.Merchants, player.BrandCode);

            var response = await _client.LoginWithoutRedirectAsync(
                configuration.ApiUrl,
                merchant.AgentId,
                merchant.AgentKey,
                new JiliLoginWithoutRedirectRequest
                {
                    GameExternalId = game.ExternalId,
                    Language = cultureCode,
                    Token = encryptedToken
                }).ConfigureAwait(false);

            if (response.ErrorCode != 0)
            {
                throw new GameProviderApiErrorException($"Get jili game url fail: {response.ErrorMessage}");
            }

            return new GetGameUrlResponse
            {
                Url = response.Url
            };
        }
    }
}
