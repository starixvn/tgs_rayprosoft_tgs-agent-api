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
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
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
        private readonly IBrandCurrencyCodeMapper _brandCurrencyCodeMapper;
        private readonly IBrandLanguageCodeMapper _brandLanguageCodeMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IRedisCacheManager _cacheManger;
        private readonly IExpirationValidator _expirationValidator;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IJsonSerializationProvider _json;
        private readonly ILocalizationOperations _localizationOperations;
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
            ILocalizationOperations localizationOperations,
            IBrandLanguageCodeMapper brandLanguageCodeMapper,
            IBrandCurrencyCodeMapper brandCurrencyCodeMapper,
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
            _localizationOperations = localizationOperations;
            _brandLanguageCodeMapper = brandLanguageCodeMapper;
            _brandCurrencyCodeMapper = brandCurrencyCodeMapper;
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
            var currencyCode = await _brandCurrencyCodeMapper.ExternalToInternalAsync(request.CurrencyCode, brandId);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);

            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var platformType = request.PlatformType;

            CachedPlayer cachedPlayer = null;

            if (playerTokenData != null)
            {
                if (playerTokenData.PlatformType != platformType)
                {
                    throw new InvalidOperationException("request platform is not matched with player login platform.");
                }

                cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerTokenData.PlayerCode);
                if (cachedPlayer != null && cachedPlayer.DefaultWalletCurrencyCode.IgnoreCaseNotEquals(currencyCode))
                {
                    throw new CurrencyNotMatchingException(cachedPlayer.DefaultWalletCurrencyCode, currencyCode);
                }
            }

            var playerType = cachedPlayer?.Type ?? PlayerType.RealPlayer;
            var cultureCode = await _brandLanguageCodeMapper.ExternalToInternalAsync(request.Language, brandId);

            var cacheType = RedisCacheType.BrandGamesWithConditions;
            var cacheKey = $"{brandId}";
            var field = $"{gameProviderId}:{platformType}:{currencyCode}:{(int) playerType}:{cultureCode}";

            var value =
                await _cacheManger.SafeGetHashSetAsync(cacheType, cacheKey, field);

            if (value.HasValue)
            {
                return new GetGamesResponse
                {
                    Games = _json.DeserializeFromString<GetGamesResponse.GameModel[]>(value)
                };
            }

            var gameModels = await GetGameModelsAsync(brandId, gameProviderId, platformType, currencyCode, cultureCode,
                playerType);

            await _cacheManger.SafeCacheHashSetAsync(cacheType, cacheKey, new HashEntry[]
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
            await _statusValidator.ThrowIfGameOrGameProviderStatusIsNotOneOfAsync(cachedPlayer.BrandId, gameProviderId,
                game.Id, gameCode,
                statuses, gameStatuses);

            var serviceUrl = await _pluggableOperations.GetEndpointUrlByGameProviderCodeAsync(gameProviderCode);
            var r = await _pluggableApiClient.GetGameUrlAsync(serviceUrl, new()
            {
                Token = playerTokenData,
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

        private async Task<GetGamesResponse.GameModel[]> GetGameModelsAsync(Guid brandId, Guid gameProviderId,
            PlatformType platformType, string currencyCode, string culture, PlayerType playerType)
        {
            var statuses = _validStatus.GetStatus(playerType);
            var gameStatuses = _validGameStatus.GetStatus(playerType);

            if (currencyCode.IsNotNullOrEmpty())
            {
                var isCurrencySupported =
                    await _gameProviderOperations.CurrencySupportedAsync(gameProviderId, currencyCode);

                if (!isCurrencySupported)
                {
                    return Array.Empty<GetGamesResponse.GameModel>();
                }
            }

            var brandGameProviderStatus =
                await _brandOperations.GetBrandGameProviderStatusAsync(brandId, gameProviderId) ?? Status.Active;
            var brandGames = await _brandOperations.GetBrandGamesAsync(brandId, platformType);
            var cachedGameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(gameProviderId);
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId);

            var store = await _localizationOperations.GetLocalizationByGameProviderIdAsync(gameProviderId, culture);

            return (from bg in brandGames.Where(g => g.GameProviderId == gameProviderId)
                    join g in cachedGames on bg.GameId equals g.Id
                    where g.AvailableCurrencies.Length == 0 ||
                        g.AvailableCurrencies.Contains(currencyCode, StringComparer.OrdinalIgnoreCase)
                    let key = g.Id.ToString()
                    select new GetGamesResponse.GameModel
                    {
                        Id = bg.GameId,
                        Code = g.Code,
                        Name = _localizationOperations.Parse(store, key,
                            Core.Models.Consts.LocalizationContext.GameName, g.Name),
                        Description = _localizationOperations.Parse(store, key,
                            Core.Models.Consts.LocalizationContext.GameDescription, g.Description),
                        GameProviderCode = cachedGameProvider.Code,
                        Type = g.Type,
                        IsActive = IsGameActive(statuses, cachedGameProvider.Status, brandGameProviderStatus,
                            gameStatuses, g.Status, bg.BrandGameStatus),
                        Tag = new GetGamesResponse.Tag
                        {
                            New = bg.Tag.IsNew(),
                            Hot = bg.Tag.IsHot(),
                            Recommend = bg.Tag.IsRecommend()
                        }
                    }).ToArray();
        }

        private static bool IsGameActive(Status[] statuses, Status gameProviderStatus, Status brandGameProviderStatus,
            GameStatus[] gameStatuses, GameStatus gameStatus, GameStatus brandGameStatus) =>
            statuses.Contains(gameProviderStatus) &&
            gameStatuses.Contains(gameStatus) &&
            statuses.Contains(brandGameProviderStatus) && gameStatuses.Contains(brandGameStatus);

        #endregion
    }
}