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
using TGS.Services.Operator.GameProviders;
using TGS.Services.PluggableServices;
using TGS.Services.PluggableServices.Models;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Game
{
    internal class GetGameUrlQueryService : IGetService<GetGameUrlQuery, GameUrlInfo>
    {
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IGameProviderRepository _gameProviderRepository;
        private readonly IPluggalbeGameProviderApiClient _pluggableApiClient;
        private readonly IPluggableOperations _pluggableOperations;
        private readonly IStatusValidator _statusValidator;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IValidStatus<Status> _validStatus;

        public GetGameUrlQueryService(
            IStatusValidator statusValidator,
            IGameProviderOperations gameProviderOperations,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus,
            IPluggableOperations pluggableOperations,
            IPluggalbeGameProviderApiClient pluggableApiClient,
            IGameProviderRepository gameProviderRepository)
        {
            _statusValidator = statusValidator ?? throw new ArgumentNullException(nameof(statusValidator));
            _gameProviderOperations =
                gameProviderOperations ?? throw new ArgumentNullException(nameof(gameProviderOperations));
            _validStatus = validStatus ?? throw new ArgumentNullException(nameof(validStatus));
            _validGameStatus = validGameStatus ?? throw new ArgumentNullException(nameof(validGameStatus));
            _pluggableOperations = pluggableOperations ?? throw new ArgumentNullException(nameof(pluggableOperations));
            _pluggableApiClient = pluggableApiClient ?? throw new ArgumentNullException(nameof(pluggableApiClient));
            _gameProviderRepository = gameProviderRepository;
        }

        public async ValueTask<GameUrlInfo> GetAsync(
            GetGameUrlQuery query,
            CancellationToken cancellationToken = default)
        {
            var playerTokenData = query.PlayerTokenData;
            var platformType = playerTokenData.PlatformType;
            var subGameProviderCode = query.SubGameProviderCode;
            var gameId = query.GameId;

            var subGp = await _gameProviderRepository
                .GetSubGameProviderDataInfoAsync(subGameProviderCode)
                .ConfigureAwait(false);

            if (subGp == null)
                throw new ArgumentException(
                    $"No game provider of code '{subGameProviderCode}' is found");

            var gameProvider = await _gameProviderOperations
                .GetCachedGameProviderAsync(subGp.GameProviderId)
                .ConfigureAwait(false);

            var gameProviderCode = gameProvider.Code;

            var statuses = _validStatus.GetStatus(query.PlayerType);
            var gameStatuses = _validGameStatus.GetStatus(query.PlayerType);

            await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(
                    query.BrandId,
                    subGp.GameProviderId,
                    gameId,
                    gameId.ToString(),
                    statuses,
                    gameStatuses)
                .ConfigureAwait(false);

            var gamePlatformType = await _gameProviderOperations
                .GetGamePlatformTypeAsync(subGp.GameProviderId, gameId)
                .ConfigureAwait(false);

            if (gamePlatformType == null)
            {
                throw new InvalidOperationException($"Game with id '{gameId}' doesn't exist");
            }

            var allowedGamePlatformTypes = platformType.ToGamePlatformTypes();
            if (!allowedGamePlatformTypes.Contains(gamePlatformType.Value))
            {
                string allowPlatforms = allowedGamePlatformTypes.JoinAsString(",");
                string msg =
                    $"Launch game platform is expected as '{allowPlatforms}' " +
                    $"but is {gamePlatformType.Value} with player login as '{platformType}'";

                throw new InvalidOperationException(msg);
            }

            var serviceUrl = await _pluggableOperations
                .GetEndpointUrlByGameProviderCodeAsync(gameProviderCode)
                .ConfigureAwait(false);

            var gameUrlResponse = await _pluggableApiClient
                .GetGameUrlAsync(
                    serviceUrl,
                    new GetGameUrlRequest
                    {
                        Token = playerTokenData,
                        GameProviderCode = gameProviderCode,
                        GameId = gameId,
                        Language = query.LanguageCode,
                        ExtendedData = query.ExtendedData
                    })
                .ConfigureAwait(false);

            return new GameUrlInfo(
                gameUrlResponse.Error,
                gameUrlResponse.ErrorMessage,
                gameUrlResponse.Url);
        }
    }
}
