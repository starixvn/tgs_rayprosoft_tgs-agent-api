using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Polly;
using Tgs.Core.Infrastructures.Security;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.GameProviders.Repositories.Models;
using TGS.Services.GameOperations;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.SexyGaming;
using TGS.Shared.Game.Models.SexyGaming.Enums;
using TGS.Shared.Game.Models.SexyGaming.Outgoing;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services
{
    public interface ISexyGamingServices
    {
        ValueTask<PlayerLoginRecordModel> DecryptTokenAsync(string token);

        ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);

        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(
            Guid gameProviderId, string playerCode, string externalTransactionId);

        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(
                                    Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType);

        ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);

        ValueTask<Guid> GetGameProviderIdAsync();

        ValueTask<SexyGamingGameUrlResponse> GetGameUrlAsync(
            SexyGamingConfiguration configuration, SexyGamingLoginGameRequest request);
    }

    internal sealed class SexyGamingServices : ISexyGamingServices
    {
        private readonly IBrandOperations _brandOperations;
        private readonly ISexyGamingApiClient _client;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IJsonSerializationProvider _jsonSerializationProvider;
        private readonly ILogger<SexyGamingServices> _logger;
        private readonly IPlayerLoginRecordService _playerLoginRecordService;
        private readonly IPlayerOperations _playerOperations;
        private readonly ITokenProvider _tokenProvider;

        public SexyGamingServices(
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IBrandOperations brandOperations,
            IPlayerOperations playerOperations,
            ITokenProvider tokenProvider,
            ISexyGamingApiClient client,
            ILogger<SexyGamingServices> logger,
            IJsonSerializationProvider jsonSerializationProvider,
            IPlayerLoginRecordService playerLoginRecordService
        )
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _jsonSerializationProvider = jsonSerializationProvider ?? throw new ArgumentNullException(nameof(jsonSerializationProvider));
            _playerLoginRecordService = playerLoginRecordService ?? throw new ArgumentNullException(nameof(playerLoginRecordService));
        }

        public async ValueTask<PlayerLoginRecordModel> DecryptTokenAsync(string token)
        {
            if (token.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }

            var loginRecordId = await _tokenProvider.DecryptAsync<Guid>(token);

            return await _playerLoginRecordService.GetAsync(loginRecordId).ConfigureAwait(false);
        }

        public async ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode)
                    => await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);

        public async ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(
            Guid gameProviderId, string playerCode, string externalTransactionId)
            => await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId)
                .ConfigureAwait(false);

        public async ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(
            Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType)
        {
            var autoCloseRound = await _gameProviderOperations.GetAutoCloseRoundAsync(gameProviderId)
                .ConfigureAwait(false);

            return new()
            {
                GameProviderId = gameProviderId,
                GameProviderConfigurationId = gameProviderConfigurationId,
                AutoCloseRound = autoCloseRound,
                PlatformType = platformType
            };
        }

        public async ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
        {
            var cachedBrandGameProviderConfiguration = await _brandOperations
                .GetBrandGameProviderConfigurationAsync(brandId, gameProviderId).ConfigureAwait(false);

            return cachedBrandGameProviderConfiguration.GameProviderConfigurationId;
        }

        public async ValueTask<Guid> GetGameProviderIdAsync() => await _gameProviderOperations
            .GetGameProviderIdByCodeAsync(GameProvider.SexyGaming).ConfigureAwait(false);

        public async ValueTask<SexyGamingGameUrlResponse> GetGameUrlAsync(
            SexyGamingConfiguration configuration, SexyGamingLoginGameRequest request)
        {
            var tryCount = 3;
            var seconds = 1;

            var retryPolicy = Policy.HandleResult<SexyGamingGameUrlResponse>(r
                    => r.Status.IgnoreCaseEquals(((int)GameApiError.UnableToProceedPleaseTryAgainLater).ToString("d4")))
                .WaitAndRetryAsync(tryCount,
                    retryAttempt => TimeSpan.FromSeconds(seconds),
                    (response, retryTime, context) =>
                    {
                        var data = new
                        {
                            GameProviderCode = GameProvider.SexyGaming,
                            Request = request,
                            Response = response,
                            Delay = retryTime,
                            CreationTime = DateTimeOffset.UtcNow
                        };
                        _logger.LogWarning(_jsonSerializationProvider.SerializeToString(data));
                    });

            return await retryPolicy.ExecuteAsync(async ()
                => await _client.GetGameUrlAsync(configuration, request).ConfigureAwait(false));
        }
    }
}
