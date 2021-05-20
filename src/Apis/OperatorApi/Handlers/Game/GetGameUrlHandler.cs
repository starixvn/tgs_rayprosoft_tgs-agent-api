using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Extensions;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;
using TGS.Services.PluggableServices;

namespace TGS.Apis.OperatorApi.Handlers.Game
{
    public sealed class GetGameUrlHandler : IRequestHandler<GetGameUrlRequest, GetGameUrlResponse>
    {
        private readonly IBrandLanguageCodeMapper _brandLanguageCodeMapper;
        private readonly IExpirationValidator _expirationValidator;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;
        private readonly IPlayerOperations _playerOperations;
        private readonly IPluggalbeGameProviderApiClient _pluggableApiClient;
        private readonly IPluggableOperations _pluggableOperations;
        private readonly IStatusValidator _statusValidator;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IValidStatus<Status> _validStatus;

        public GetGameUrlHandler(
            IHttpContextAccessor httpContextAccessor,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandLanguageCodeMapper brandLanguageCodeMapper,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus,
            IStatusValidator statusValidator,
            IExpirationValidator expirationValidator,
            IPluggalbeGameProviderApiClient pluggableApiClient,
            IPluggableOperations pluggableOperations)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _brandLanguageCodeMapper = brandLanguageCodeMapper;
            _validStatus = validStatus;
            _validGameStatus = validGameStatus;
            _statusValidator = statusValidator;
            _expirationValidator = expirationValidator;
            _pluggableApiClient = pluggableApiClient;
            _pluggableOperations = pluggableOperations;
        }

        public async Task<GetGameUrlResponse> Handle(
            GetGameUrlRequest request, CancellationToken cancellationToken)
        {
            var playerTokenData = _httpContext.Items.GetPlayerTokenData();

            var platformType = playerTokenData.PlatformType;
            var playerCode = playerTokenData.PlayerCode;
            var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerCode);

            _statusValidator.ThrowIfPlayerDisabled(cachedPlayer.Status);
            await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerCode, platformType);

            var gameProviderCode = request.GameProviderCode;
            var gameId = request.GameId;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);

            var playerType = cachedPlayer.Type;
            var statuses = _validStatus.GetStatus(playerType);
            var gameStatuses = _validGameStatus.GetStatus(playerType);
            await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(cachedPlayer.BrandId, gameProviderId,
                gameId, gameId.ToString(), statuses, gameStatuses);

            var gamePlatformType = await _gameProviderOperations.GetGamePlatformTypeAsync(gameProviderId, gameId);
            if (gamePlatformType == null)
            {
                throw new InvalidOperationException($"Game with id '{gameId}' doesn't exist");
            }

            var allowedGamePlatformTypes = platformType.ToGamePlatformTypes();
            if (!allowedGamePlatformTypes.Contains(gamePlatformType.Value))
            {
                throw new InvalidOperationException(
                    $"Launch game platform is expected as '{allowedGamePlatformTypes.JoinAsString(",")}' but is {gamePlatformType.Value} with player login as '{platformType}'");
            }

            var language =
                await _brandLanguageCodeMapper.ExternalToInternalAsync(request.LanguageCode, cachedPlayer.BrandId);
            var serviceUrl = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(gameProviderCode);
            var r = await _pluggableApiClient.GetGameUrlAsync(serviceUrl, new()
            {
                Token = playerTokenData,
                GameProviderCode = gameProviderCode,
                GameId = gameId,
                Language = language,
                ExtendedData = request.ExtendedData
            });

            return new()
            {
                Error = r.Error,
                ErrorMessage = r.ErrorMessage,
                Url = r.Url
            };
        }
    }
}