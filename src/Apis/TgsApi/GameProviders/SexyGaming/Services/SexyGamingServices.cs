using System;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.GameProviders.Settings;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Data;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;
using TGS.Services.Operations.Queries;
using TGS.Shared.Game.ApiClients.SexyGaming;
using TGS.Shared.Game.Models.SexyGaming.Enums;
using TGS.Shared.Game.Models.SexyGaming.Outgoing;

namespace TGS.Apis.TgsApi.GameProviders.SexyGaming.Services
{
    public interface ISexyGamingServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType);
        ValueTask<GameActionExternalIds> GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId);
        ValueTask<Guid> GetGameProviderId();
        ValueTask<Guid> GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId);
        ValueTask<Guid> GetBrandIdByPlayerCodeAsync(string playerCode);
        ValueTask<CachedPlayerLoginRecord> DecryptTokenAsync(string token);
        ValueTask<SexyGamingGameUrlResponse> GetGameUrlAsync(SexyGamingConfiguration configuration, SexyGamingLoginGameRequest request);
    }
    internal sealed class SexyGamingServices : ISexyGamingServices
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameActionQueries _gameActionQueries;
        private readonly IBrandOperations _brandOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ITokenProvider _tokenProvider;
        private readonly ISexyGamingApiClient _client;
        public SexyGamingServices(
            IGameProviderOperations gameProviderOperations,
            IGameActionQueries gameActionQueries,
            IBrandOperations brandOperations,
            IPlayerOperations playerOperations,
            ITokenProvider tokenProvider,
            ISexyGamingApiClient client
            )
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _gameActionQueries = gameActionQueries ?? throw new ArgumentNullException(nameof(gameActionQueries));
            _brandOperations = brandOperations ?? throw new ArgumentNullException(nameof(brandOperations));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _tokenProvider = tokenProvider ?? throw new ArgumentNullException(nameof(tokenProvider));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        async ValueTask<Guid> ISexyGamingServices.GetBrandIdByPlayerCodeAsync(string playerCode)
            => await _playerOperations.GetBrandIdByPlayerCodeAsync(playerCode);

        async ValueTask<GameActionExternalIds> ISexyGamingServices.GetGameActionExternalIdAsync(Guid gameProviderId, string playerCode, string externalTransactionId)
            => await _gameActionQueries.GetGameActionExternalIdAsync(gameProviderId, playerCode, externalTransactionId).ConfigureAwait(false);

        async ValueTask<GameProviderConfigurationData> ISexyGamingServices.GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType)
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

        async ValueTask<Guid> ISexyGamingServices.GetGameProviderConfigurationIdAsync(Guid brandId, Guid gameProviderId)
        {
            var cachedBrandGameProviderConfiguration = await _brandOperations.GetBrandGameProviderConfigurationAsync(brandId, gameProviderId).ConfigureAwait(false);
            return cachedBrandGameProviderConfiguration.GameProviderConfigurationId;
        }

        async ValueTask<Guid> ISexyGamingServices.GetGameProviderId()
        {
            return await _gameProviderOperations.GetGameProviderIdByCodeAsync(Consts.GameProvider.SexyGaming).ConfigureAwait(false);
        }

        async ValueTask<CachedPlayerLoginRecord> ISexyGamingServices.DecryptTokenAsync(string token)
        {
            if (token.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }

            var loginRecordId = await _tokenProvider.DecryptAsync<Guid>(token);
            return await _playerOperations.GetPlayerLoginRecordAsync(loginRecordId).ConfigureAwait(false);
        }

        async ValueTask<SexyGamingGameUrlResponse> ISexyGamingServices.GetGameUrlAsync(SexyGamingConfiguration configuration, SexyGamingLoginGameRequest request)
        {
            int count = 0;
            int tryCount = 3;
            while (count < tryCount)
            {
                var response = await _client.GetGameUrlAsync(configuration, request).ConfigureAwait(false);

                if (response.Status.IgnoreCaseEquals(((int)GameApiError.UnableToProceedPleaseTryAgainLater).ToString("d4")))
                {
                    count++;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
                else
                {
                    return response;
                }
            }
            throw new GameProviderApiErrorException("Failed to execute 3 times to obtain SexyGaming game URL.(取得SexyGaming遊戲網址執行3次失敗。)");
        }
    }
}
