using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Security;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Services.Caches;
using TGS.Core.Models.Services.Games;
using TGS.Services.Operations;

namespace TGS.Apis.TgsApi.GameProviders.Skywind.Services
{
    public interface ISkywindServices
    {
        ValueTask<GameProviderConfigurationData> GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType);
        ValueTask<CachedPlayerLoginRecord> DecryptTokenAsync(string token);
    }

    public class SkywindServices : ISkywindServices
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly ITokenProvider _tokenProvider;
        private readonly IPlayerOperations _playerOperations;

        public SkywindServices(
            IGameProviderOperations gameProviderOperations,
             ITokenProvider _tokenProvider,
             IPlayerOperations playerOperations)
        {
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _tokenProvider = _tokenProvider ?? throw new ArgumentNullException(nameof(_tokenProvider));
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
        }

        async ValueTask<GameProviderConfigurationData> ISkywindServices.GetGameProviderConfigurationDataAsync(Guid gameProviderId, Guid gameProviderConfigurationId, PlatformType platformType)
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

        async ValueTask<CachedPlayerLoginRecord> ISkywindServices.DecryptTokenAsync(string token)
        {
            if (token.IsNullOrEmpty())
            {
                throw new MissingTokenException("empty token");
            }

            var loginRecordId = await _tokenProvider.DecryptAsync<Guid>(token);
            return await _playerOperations.GetPlayerLoginRecordAsync(loginRecordId);
        }
    }
}
