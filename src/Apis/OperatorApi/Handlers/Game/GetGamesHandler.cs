using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using StackExchange.Redis;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Exceptions;
using TGS.Core.Models.Extensions;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using Localization = TGS.Core.Models.Consts.LocalizationContext;

namespace TGS.Apis.OperatorApi.Handlers.Game
{
    public sealed class GetGamesHandler : IRequestHandler<GetGamesRequest, GetGamesResponse>
    {
        private readonly IBrandCurrencyCodeMapper _brandCurrencyCodeMapper;
        private readonly IBrandLanguageCodeMapper _brandLanguageCodeMapper;
        private readonly IBrandOperations _brandOperations;
        private readonly IRedisCacheManager _cacheManger;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly HttpContext _httpContext;
        private readonly IJsonSerializationProvider _json;
        private readonly ILocalizationOperations _localizationOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly IValidStatus<GameStatus> _validGameStatus;
        private readonly IValidStatus<Status> _validStatus;

        public GetGamesHandler(
            IHttpContextAccessor httpContextAccessor,
            IBrandOperations brandOperations,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            ILocalizationOperations localizationOperations,
            IBrandLanguageCodeMapper brandLanguageCodeMapper,
            IBrandCurrencyCodeMapper brandCurrencyCodeMapper,
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus)
        {
            _httpContext = httpContextAccessor.HttpContext;
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
        }

        async Task<GetGamesResponse> IRequestHandler<GetGamesRequest, GetGamesResponse>.Handle(GetGamesRequest request,
            CancellationToken cancellationToken)
        {
            var brandId = _httpContext.Items.GetBrandId();
            var currencyCode = await _brandCurrencyCodeMapper.ExternalToInternalAsync(request.CurrencyCode, brandId);
            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);

            var playerTokenData = _httpContext.Items.GetPlayerTokenData();
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
            var cultureCode = await _brandLanguageCodeMapper.ExternalToInternalAsync(request.LanguageCode, brandId);

            const RedisCacheType cacheType = RedisCacheType.BrandGamesWithConditions;
            var cacheKey = brandId;
            var field = $"{gameProviderId}:{platformType}:{currencyCode}:{(int)playerType}";

            var value =
                await _cacheManger.SafeGetHashSetAsync(cacheType, cacheKey, field);

            GameModel[] games;

            if (value.HasValue)
            {
                games = _json.DeserializeFromString<GameModel[]>(value);
            }
            else
            {
                games = await GetGameModelsAsync(brandId, gameProviderId, platformType, currencyCode,
                    playerType);

                await _cacheManger.SafeCacheHashSetAsync(cacheType, cacheKey,
                    new HashEntry[] {new(field, _json.SerializeToString(games))}, TimeSpan.FromHours(2));
            }

            var types = request.Types ?? Array.Empty<GameType>();
            var store = await _localizationOperations.GetLocalizationByGameProviderIdAsync(gameProviderId, cultureCode);

            var translatedGames = games
                .Where(g => types.Length == 0 || types.Contains(g.Type))
                .Select(g =>
                {
                    var key = g.Id.ToString();

                    return g with
                    {
                        Name = _localizationOperations.Parse(store, key,
                            Localization.GameName, g.Name),
                        Description = _localizationOperations.Parse(store, key,
                            Localization.GameDescription, g.Description)
                    };
                }).ToArray();

            return new() {Games = translatedGames};
        }

        private async Task<GameModel[]> GetGameModelsAsync(Guid brandId, Guid gameProviderId,
            PlatformType platformType, string currencyCode, PlayerType playerType)
        {
            var statuses = _validStatus.GetStatus(playerType);
            var gameStatuses = _validGameStatus.GetStatus(playerType);

            if (currencyCode.IsNotNullOrEmpty())
            {
                var isCurrencySupported =
                    await _gameProviderOperations.CurrencySupportedAsync(gameProviderId, currencyCode);

                if (!isCurrencySupported)
                {
                    return Array.Empty<GameModel>();
                }
            }

            var gamePlatformTypes = platformType.ToGamePlatformTypes();
            var brandGameProviderStatus =
                await _brandOperations.GetBrandGameProviderStatusAsync(brandId, gameProviderId) ?? Status.Active;
            var brandGames = await _brandOperations.GetBrandGamesAsync(brandId, gamePlatformTypes);
            var cachedGameProvider = await _gameProviderOperations.GetCachedGameProviderAsync(gameProviderId);
            var cachedGames = await _gameProviderOperations.GetCachedGamesAsync(gameProviderId);

            return (from bg in brandGames.Where(g => g.GameProviderId == gameProviderId)
                    join g in cachedGames on bg.GameId equals g.Id
                    where g.AvailableCurrencies.Length == 0 ||
                        g.AvailableCurrencies.Contains(currencyCode, StringComparer.OrdinalIgnoreCase)
                    select new GameModel
                    {
                        Id = bg.GameId,
                        Code = g.Code,
                        Name = g.Name,
                        Description = g.Description,
                        GameProviderCode = cachedGameProvider.Code,
                        Type = g.Type,
                        IsActive = IsGameActive(statuses, cachedGameProvider.Status, brandGameProviderStatus,
                            gameStatuses, g.Status, bg.BrandGameStatus),
                        Tag = new()
                        {
                            New = bg.Tag.IsNew(),
                            Hot = bg.Tag.IsHot(),
                            Recommend = bg.Tag.IsRecommend()
                        }
                    }).ToArray();
        }

        private static bool IsGameActive(Status[] statuses, Status gameProviderStatus, Status brandGameProviderStatus,
            GameStatus[] gameStatuses, GameStatus gameStatus, GameStatus brandGameStatus) =>
            statuses.Contains(gameProviderStatus) && gameStatuses.Contains(gameStatus) &&
            statuses.Contains(brandGameProviderStatus) && gameStatuses.Contains(brandGameStatus);
    }
}