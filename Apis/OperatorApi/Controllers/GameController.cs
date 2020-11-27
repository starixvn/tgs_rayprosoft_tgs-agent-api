using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages.Games;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Clients;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class GameController : ControllerBase
    {
        private readonly IRedisCacheManager _cacheManger;
        private readonly IExpirationValidator _expirationValidator;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IJsonSerializationProvider _json;
        private readonly IBrandLanguageCodeMapper _brandLanguageCodeMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IPluggalbeGameProviderApiClient _pluggableApiClient;
        private readonly IPluggableOperations _pluggableOperations;
        private readonly IStatusValidator _statusValidator;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IValidStatus<Status> _validStatus;

        public GameController(
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            IBrandLanguageCodeMapper brandLanguageCodeMapper,
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus,
            IStatusValidator statusValidator,
            IExpirationValidator expirationValidator,
            IPluggalbeGameProviderApiClient pluggableApiClient,
            IPluggableOperations pluggableOperations)
        {
            _brandOperations = brandOperations;
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _brandLanguageCodeMapper = brandLanguageCodeMapper;
            _cacheManger = cacheManager;
            _json = json;
            _validStatus = validStatus;
            _validGameStatus = validGameStatus;
            _statusValidator = statusValidator;
            _expirationValidator = expirationValidator;
            _pluggableApiClient = pluggableApiClient;
            _pluggableOperations = pluggableOperations;
        }

        [HttpPost]
        [Route("list")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public async Task<GetGamesResponse> GetGamesAsync(GetGamesRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var cultureCode = await _brandLanguageCodeMapper.ExternalToInternalAsync(request.Language, brandId);

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);

            var platform = request.PlatformType;

            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var cachedPlayer = playerTokenData != null
                ? await _playerOperations.GetCachedPlayerAsync(playerTokenData.PlayerCode)
                : null;

            var cacheKey = $"{brandId}";
            var field = cachedPlayer == null
                ? $"{platform}:{gameProviderId}:{cultureCode}:general"
                : $"{platform}:{gameProviderId}:{cultureCode}:{cachedPlayer.DefaultWalletCurrencyCode}:{(int) cachedPlayer.Type}";

            var value =
                await _cacheManger.SafeGetHashSetAsync(RedisCacheType.BrandGamesByPlatform, cacheKey, field);

            if (value.HasValue)
            {
                return new GetGamesResponse
                {
                    Games = _json.DeserializeFromString<GetGamesResponse.GameModel[]>(value)
                };
            }

            var brandGames = await _brandOperations.GetBrandGamesAsync(brandId, platform);
            var gameModels = await GetGameModelsAsync(gameProviderId, brandGames, cachedPlayer);

            await _cacheManger.SafeCacheHashSetAsync(RedisCacheType.BrandGamesByPlatform, cacheKey, new HashEntry[]
            {
                new(field, _json.SerializeToString(gameModels))
            }, TimeSpan.FromHours(1));

            return new GetGamesResponse
            {
                Games = gameModels
            };
        }

        [HttpPost]
        [Route("geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<GetGameUrlResponse> GetGameUrlAsync(GetGameUrlRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var platformType = playerTokenData.PlatformType;
            var playerCode = playerTokenData.PlayerCode;
            var cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerCode);

            _statusValidator.ThrowIfPlayerDisabled(cachedPlayer.Status);
            await _expirationValidator.ThrowIfPlayerLoginExpiredAsync(playerCode, platformType);

            var gameProviderCode = request.GameProviderCode;
            var gameCode = request.GameCode;
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(gameProviderCode);
            var gameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(gameProviderId);
            var game = await _gameProviderOperations.GetCachedGameAsync(gameProviderId, gameCode, platformType);

            var playerType = cachedPlayer.Type;
            var statuses = _validStatus.GetStatus(playerType);
            var gameStatuses = _validGameStatus.GetStatus(playerType);
            await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(cachedPlayer.BrandId, gameProviderId, game.Id, gameCode,
                statuses, gameStatuses);

            var serviceUrl = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(gameProviderCode);
            var r = await _pluggableApiClient.GetGameUrlAsync(serviceUrl, new()
            {
                Player = cachedPlayer,
                GameProvider = gameProvider,
                Game = game
            });

            return new GetGameUrlResponse
            {
                Error = r.Error,
                ErrorMessage = r.ErrorMessage,
                Url = r.Url
            };
        }

        #region Private Methods

        private async Task<GetGamesResponse.GameModel[]> GetGameModelsAsync(Guid gameProviderId,
            CachedBrandGame[] licenseeGames, CachedPlayer cachedPlayer)
        {
            var statuses = ValidStatus.Default;
            var gameStatuses = ValidGameStatus.Default;
            var currencyCode = string.Empty;

            if (cachedPlayer != null)
            {
                statuses = _validStatus.GetStatus(cachedPlayer.Type);
                gameStatuses = _validGameStatus.GetStatus(cachedPlayer.Type);
                currencyCode = cachedPlayer.DefaultWalletCurrencyCode;
            }

            if (currencyCode.IsNotNullOrEmpty())
            {
                var isCurrencySupported =
                    await _gameProviderOperations.CurrencySupportedAsync(gameProviderId, currencyCode);

                if (!isCurrencySupported)
                {
                    return Array.Empty<GetGamesResponse.GameModel>();
                }
            }

            var cachedGameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(gameProviderId);
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId);

            return (from lg in licenseeGames.Where(g => g.GameProviderId == gameProviderId)
                    join gg in cachedGames on lg.GameId equals gg.Id
                    select new GetGamesResponse.GameModel
                    {
                        Id = lg.GameId,
                        Code = gg.Code,
                        Name = gg.Name,
                        Description = gg.Description,
                        GameProviderCode = cachedGameProvider.Code,
                        Type = gg.Type,
                        IsActive = statuses.Contains(cachedGameProvider.Status) && gameStatuses.Contains(gg.Status)
                    }).ToArray();
        }

        #endregion
    }
}