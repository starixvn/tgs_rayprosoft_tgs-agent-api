using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages.Games;
using TGS.Core.Infrastructures.Extensions;
using TGS.Core.Infrastructures.Json;
using TGS.Core.Infrastructures.Redis;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Services.Caches;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous, IgnoreAntiforgeryToken]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class GameController : ControllerBase
    {
        private readonly ILicenseeOperations _licenseeOperations;
        private readonly IGameProviderOperations _gameProviderOperations;
        private readonly IPlayerOperations _playerOperations;
        private readonly ILicenseeLanguageCodeMapper _licenseeLanguageCodeMapper;
        private readonly IRedisCacheManager _cacheManger;
        private readonly IJsonSerializationProvider _json;
        private readonly IValidStatus<Status> _validStatus;
        private readonly IValidStatus<GameStatus> _validGameStatus;

        public GameController(
            ILicenseeOperations licenseeOperations,
            IGameProviderOperations gameProviderOperations,
            IPlayerOperations playerOperations,
            ILicenseeLanguageCodeMapper licenseeLanguageCodeMapper,
            IRedisCacheManager cacheManager,
            IJsonSerializationProvider json,
            IValidStatus<Status> validStatus,
            IValidStatus<GameStatus> validGameStatus)
        {
            _licenseeOperations = licenseeOperations;
            _gameProviderOperations = gameProviderOperations;
            _playerOperations = playerOperations;
            _licenseeLanguageCodeMapper = licenseeLanguageCodeMapper;
            _cacheManger = cacheManager;
            _json = json;
            _validStatus = validStatus;
            _validGameStatus = validGameStatus;
        }

        [HttpPost, Route("list")]
        public async Task<GetGamesResponse> GetGamesAsync(GetGamesRequest request)
        {
            var licenseeId = HttpContext.Items.GetLicenseeId();
            var cultureCode = await _licenseeLanguageCodeMapper.ExternalToInternalAsync(request.Language, licenseeId);

            var gameProviderId = await _gameProviderOperations.GetGameProviderIdByCodeAsync(request.GameProviderCode);
            
            var platform = request.PlatformType;
            var username = request.UserName;
            CachedPlayer cachedPlayer = null;
            if (username.IsNotNullOrEmpty())
            {
                var playerCode = await _playerOperations.GetPlayerCodeByUsernameAsync(licenseeId, username);
                cachedPlayer = await _playerOperations.GetCachedPlayerAsync(playerCode);
            }

            var cacheKey = $"{licenseeId}";
            var field = cachedPlayer == null
                ? $"{platform}:{gameProviderId}:{cultureCode}:general:"
                : $"{platform}:{gameProviderId}:{cultureCode}:{cachedPlayer.DefaultWalletCurrencyCode}:{(int)cachedPlayer.Type}";
            
            var value =
                await _cacheManger.SafeGetHashSetAsync(RedisCacheType.LicenseeGamesByPlatform, licenseeId, field);

            if (value.HasValue)
            {
                return new GetGamesResponse
                {
                    Games = _json.DeserializeFromString<GetGamesResponse.GameModel[]>(value)
                };
            }

            var licenseeGames = await _licenseeOperations.GetLicenseeGamesAsync(licenseeId, platform);
            var gameModels = await GetGameModelsAsync(gameProviderId, licenseeGames, cachedPlayer);

            await _cacheManger.SafeCacheHashSetAsync(RedisCacheType.LicenseeGamesByPlatform, cacheKey, new HashEntry[]
            {
                new(field, _json.SerializeToString(gameModels))
            }, TimeSpan.FromHours(1));

            return new GetGamesResponse
            {
                Games = gameModels
            };
        }

        private async Task<GetGamesResponse.GameModel[]> GetGameModelsAsync(Guid gameProviderId, CachedLicenseeGame[] licenseeGames, CachedPlayer cachedPlayer)
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
    }
}
