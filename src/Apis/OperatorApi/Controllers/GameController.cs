using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Game;
using TGS.Services.Operator.Services.GameBalance;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class GameController : ControllerBase
    {
        /// <summary>
        ///     get game list of specific game provider
        /// </summary>
        [HttpGet("list")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public async IAsyncEnumerable<GameModel> GetGamesAsync(
            [FromServices] IBrandLanguageCodeMapper brandLanguageCodeMapper,
            [FromServices] IQueryService<GetGamesQuery, GameInfo> getGamesQueryService,
            [FromServices] IQueryService<GetAllGamesByGameTypeQuery, GameInfo> queryAllGameService,
            [FromQuery] GetGamesRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var currentCultureInfo = CultureInfo.CurrentUICulture;

            var cultureCode = await brandLanguageCodeMapper
                .ExternalToInternalAsync(currentCultureInfo.Name, brandId)
                .ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(request.SubGameProviderCode))
            {
                var query = new GetAllGamesByGameTypeQuery(
                    brandId,
                    playerTokenData,
                    request.PlatformType,
                    request.Types ?? Array.Empty<GameType>());

                var games = from info in queryAllGameService.QueryAsync(query)
                            let lang = info.Localization.ContainsKey(cultureCode)
                                ? info.Localization[cultureCode]
                                : null
                            orderby info.DisplayOrder, info.Code
                            select new GameModel
                            {
                                Code = info.Code,
                                GameProviderCode = info.GameProviderCode,
                                Description = GetLocalization(
                                    lang.RootElement,
                                    "description",
                                    info.Description),
                                DisplayOrder = info.DisplayOrder,
                                Id = info.Id,
                                IsActive = info.IsActive,
                                Name = GetLocalization(
                                    lang.RootElement,
                                    "name",
                                    info.Name),
                                Tag = new Tag
                                {
                                    Hot = info.Tag.Hot,
                                    New = info.Tag.New,
                                    Recommend = info.Tag.Recommend
                                },
                                Type = info.Type
                            };

                await foreach (var game in games)
                    yield return game;
            }
            else
            {
                var query = new GetGamesQuery(
                    brandId,
                    request.SubGameProviderCode,
                    playerTokenData,
                    request.PlatformType,
                    request.Types ?? Array.Empty<GameType>());

                var games = from info in getGamesQueryService.QueryAsync(query)
                            let lang = info.Localization.ContainsKey(cultureCode)
                                ? info.Localization[cultureCode]
                                : null
                            orderby info.DisplayOrder, info.Code
                            select new GameModel
                            {
                                Code = info.Code,
                                GameProviderCode = info.GameProviderCode,
                                Description = GetLocalization(
                                    lang.RootElement,
                                    "description",
                                    info.Description),
                                DisplayOrder = info.DisplayOrder,
                                Id = info.Id,
                                IsActive = info.IsActive,
                                Name = GetLocalization(
                                    lang.RootElement,
                                    "name",
                                    info.Name),
                                Tag = new Tag
                                {
                                    Hot = info.Tag.Hot,
                                    New = info.Tag.New,
                                    Recommend = info.Tag.Recommend
                                },
                                Type = info.Type
                            };

                await foreach (var game in games)
                    yield return game;
            }
        }

        /// <summary>
        ///     get game launch url via game id
        /// </summary>
        [HttpPost("geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<GetGameUrlResponse> GetGameUrlForPostAsync(
            [FromServices] IPlayerOperations playerOperations,
            [FromServices] IStatusValidator playerStatusValidator,
            [FromServices] IBrandLanguageCodeMapper brandLanguageCodeMapper,
            [FromServices] IGetService<GetGpCodeQuery, string> getGpCodeService,
            [FromServices] IGetService<GetGameUrlQuery, GameUrlInfo> getGameUrlQueryService,
            [FromServices] ICommandService<FundInBeforeEnterGameCommand> fundService,
            [FromBody] GetGameUrlRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var cachedPlayer = await playerOperations
                .GetCachedPlayerAsync(playerTokenData.PlayerCode)
                .ConfigureAwait(false);

            playerStatusValidator.ThrowIfPlayerDisabled(cachedPlayer.Status);

            var currentCultureInfo = CultureInfo.CurrentUICulture;
            var language = await brandLanguageCodeMapper
                .ExternalToInternalAsync(currentCultureInfo.Name, cachedPlayer.BrandId)
                .ConfigureAwait(false);

            var gpCode = await getGpCodeService
                .GetAsync(new GetGpCodeQuery(request.SubGameProviderCode))
                .ConfigureAwait(false);

            var query = new GetGameUrlQuery(
                cachedPlayer.BrandId,
                playerTokenData,
                cachedPlayer.Type,
                request.SubGameProviderCode,
                request.GameId,
                language,
                request.ExtendedData);

            var info = await getGameUrlQueryService
                .GetAsync(query)
                .ConfigureAwait(false);

            await fundService
                .ExecuteAsync(new FundInBeforeEnterGameCommand(
                    cachedPlayer.BrandId,
                    playerTokenData.PlayerCode,
                    gpCode))
                .ConfigureAwait(false);

            return new GetGameUrlResponse
            {
                Error = info.Error,
                ErrorMessage = info.ErrorMessage,
                Url = info.Url
            };
        }

        /// <summary>
        ///     get game launch url for lobby type game.
        /// </summary>
        [HttpPost("lobby/geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<GetGameUrlResponse> GetLobbyGameUrlForPostAsync(
            [FromServices] IPlayerOperations playerOperations,
            [FromServices] IStatusValidator playerStatusValidator,
            [FromServices] IBrandLanguageCodeMapper brandLanguageCodeMapper,
            [FromServices] IGetService<GetGpCodeQuery, string> getGpCodeService,
            [FromServices] IGetService<GetLobbyGameUrlQuery, LobbyGameUrlInfo> getLobbyGameUrlQueryService,
            [FromServices] ICommandService<FundInBeforeEnterGameCommand> fundService,
            [FromBody] GetLobbyGameUrlRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var cachedPlayer = await playerOperations
                .GetCachedPlayerAsync(playerTokenData.PlayerCode)
                .ConfigureAwait(false);

            playerStatusValidator.ThrowIfPlayerDisabled(cachedPlayer.Status);

            var gpCode = await getGpCodeService
                .GetAsync(new GetGpCodeQuery(request.SubGameProviderCode))
                .ConfigureAwait(false);

            var currentCultureInfo = CultureInfo.CurrentUICulture;
            var language = await brandLanguageCodeMapper
                .ExternalToInternalAsync(currentCultureInfo.Name, cachedPlayer.BrandId)
                .ConfigureAwait(false);

            var query = new GetLobbyGameUrlQuery(
                cachedPlayer.BrandId,
                request.SubGameProviderCode,
                playerTokenData,
                cachedPlayer.Type,
                request.GameType,
                language,
                request.Extends);

            var info = await getLobbyGameUrlQueryService
                .GetAsync(query)
                .ConfigureAwait(false);

            await fundService
                .ExecuteAsync(new FundInBeforeEnterGameCommand(
                    cachedPlayer.BrandId,
                    playerTokenData.PlayerCode,
                    gpCode))
                .ConfigureAwait(false);

            return new GetGameUrlResponse
            {
                Error = info.Error,
                ErrorMessage = info.ErrorMessage,
                Url = info.Url
            };
        }

        private static string GetLocalization(JsonElement element, string propertyName, string defaultValue)
        {
            if (!element.TryGetProperty(propertyName, out var value))
            {
                return defaultValue;
            }

            var val = value.GetString();
            return string.IsNullOrEmpty(val) ? defaultValue : val;
        }
    }
}
