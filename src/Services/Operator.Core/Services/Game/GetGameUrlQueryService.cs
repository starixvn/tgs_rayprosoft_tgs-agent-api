using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class GetGameUrlQueryService : IGetService<GetGameUrlQuery, GameUrlInfo>
    {
        private readonly IPlayerOperations _playerOperations;
        private readonly IStatusValidator _statusValidator;
        private readonly IExpirationValidator _expirationValidator;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IValidStatus<Status> _validStatus;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IBrandLanguageCodeMapper _brandLanguageCodeMapper;
        private readonly IPluggableOperations _pluggableOperations;
        private readonly IPluggalbeGameProviderApiClient _pluggableApiClient;

        public GetGameUrlQueryService(
            IPlayerOperations playerOperations,
            IStatusValidator statusValidator,
            IExpirationValidator expirationValidator,
            IGameProviderOperations gameProviderOperations,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus,
            IBrandLanguageCodeMapper brandLanguageCodeMapper,
            IPluggableOperations pluggableOperations,
            IPluggalbeGameProviderApiClient pluggableApiClient)
        {
            _playerOperations = playerOperations ?? throw new ArgumentNullException(nameof(playerOperations));
            _statusValidator = statusValidator ?? throw new ArgumentNullException(nameof(statusValidator));
            _expirationValidator = expirationValidator ?? throw new ArgumentNullException(nameof(expirationValidator));
            _gameProviderOperations = gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _validStatus = validStatus ?? throw new ArgumentNullException(nameof(validStatus));
            _validGameStatus = validGameStatus ?? throw new ArgumentNullException(nameof(validGameStatus));
            _brandLanguageCodeMapper = brandLanguageCodeMapper ?? throw new ArgumentNullException(nameof(brandLanguageCodeMapper));
            _pluggableOperations = pluggableOperations ?? throw new ArgumentNullException(nameof(pluggableOperations));
            _pluggableApiClient = pluggableApiClient ?? throw new ArgumentNullException(nameof(pluggableApiClient));
        }

        public async ValueTask<GameUrlInfo> GetAsync(GetGameUrlQuery query, CancellationToken cancellationToken = default)
        {
            var playerTokenData = query.PlayerTokenData;
            var platformType = playerTokenData.PlatformType;
            var gameProviderCode = query.GameProviderCode;
            var gameId = query.GameId;

            var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerTokenData.PlayerCode);

            _statusValidator.ThrowIfPlayerDisabled(cachedPlayer.Status);
            await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerTokenData.PlayerCode, platformType);

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);

            var playerType = cachedPlayer.Type;
            var statuses = _validStatus.GetStatus(playerType);
            var gameStatuses = _validGameStatus.GetStatus(playerType);
            await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                cachedPlayer.BrandId,
                gameProviderId,
                gameId,
                gameId.ToString(),
                statuses,
                gameStatuses);

            var gamePlatformType = await _gameProviderOperations.GetGamePlatformTypeAsync(gameProviderId, gameId);
            if (gamePlatformType == null)
            {
                throw new InvalidOperationException($"Game with id '{gameId}' doesn't exist");
            }

            var allowedGamePlatformTypes = platformType.ToGamePlatformTypes();
            if (!allowedGamePlatformTypes.Contains(gamePlatformType.Value))
            {
                string allowPlatforms = allowedGamePlatformTypes.JoinAsString(",");
                string msg = $"Launch game platform is expected as '{allowPlatforms}' but is {gamePlatformType.Value} with player login as '{platformType}'";
                throw new InvalidOperationException(msg);
            }

            var language = await _brandLanguageCodeMapper.ExternalToInternalAsync(query.LanguageCode, cachedPlayer.BrandId);
            var serviceUrl = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(gameProviderCode);
            var gameUrlResponse = await _pluggableApiClient.GetGameUrlAsync(
                serviceUrl,
                new GetGameUrlRequest()
                {
                    Token = playerTokenData,
                    GameProviderCode = gameProviderCode,
                    GameId = gameId,
                    Language = language,
                    ExtendedData = query.ExtendedData
                });

            return new GameUrlInfo(
                gameUrlResponse.Error,
                gameUrlResponse.ErrorMessage,
                gameUrlResponse.Url);
        }
    }
}