using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models.Enums;
using TGS.Services.Operations;
using TGS.Services.Operations.CodeMappers;
using TGS.Services.Operations.Validators;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Game;
using TGS.Services.Operator.Services.GameBalance;
using TGS.ServiceUtility;
using Tag = TGS.Apis.OperatorApi.Models.Messages.Tag;

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
            [FromServices] IPlayerOperations playerOperations,
            [FromQuery] GetGamesRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var currentCultureInfo = CultureInfo.CurrentUICulture;

            var cultureCode = await brandLanguageCodeMapper
                .ExternalToInternalAsync(currentCultureInfo.Name, brandId)
                .ConfigureAwait(false);

            var playerType = PlayerType.RealPlayer;
            var platformType = request.PlatformType;

            if (playerTokenData != null)
            {
                var cachedPlayer = await playerOperations
                    .GetCachedPlayerAsync(playerTokenData.PlayerCode)
                    .ConfigureAwait(false);

                playerType = cachedPlayer.Type;
                platformType = playerTokenData.PlatformType;
            }

            if (string.IsNullOrWhiteSpace(request.SubGameProviderCode))
            {
                var query = new GetAllGamesByGameTypeQuery(
                    brandId,
                    playerType,
                    platformType,
                    request.Types ?? Array.Empty<GameType>());

                var games = from info in queryAllGameService.QueryAsync(query)
                            let lang = info.Localization.ContainsKey(cultureCode)
                                ? info.Localization[cultureCode]
                                : JsonDocument.Parse("{}")
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
                    playerType,
                    platformType,
                    request.Types ?? Array.Empty<GameType>());

                var games = from info in getGamesQueryService.QueryAsync(query)
                            let lang = info.Localization.ContainsKey(cultureCode)
                                ? info.Localization[cultureCode]
                                : JsonDocument.Parse("{}")
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
        /// get game launch url via game id
        /// </summary>
        [HttpPost("geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [PlayerCheckEnabled]
        public async Task<GetGameUrlResponse> GetGameUrlForPostAsync(
            [FromServices] IPlayerOperations playerOperations,
            [FromServices] IStatusValidator playerStatusValidator,
            [FromServices] IBrandLanguageCodeMapper brandLanguageCodeMapper,
            [FromServices] IGetService<GetGpCodeQuery, BrandSubGameProviderInfo> getGpCodeService,
            [FromServices] IGetService<GetGameUrlQuery, GameUrlInfo> getGameUrlQueryService,
            [FromServices] ICommandService<FundOutBeforeEnterGameCommand> fundService,
            [FromBody] GetGameUrlRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var cachedPlayer = await playerOperations
                .GetCachedPlayerAsync(playerTokenData.PlayerCode)
                .ConfigureAwait(false);

            playerStatusValidator.ThrowIfPlayerDisabled(cachedPlayer.Status);

            var currentCultureInfo = CultureInfo.CurrentUICulture;
            var language = await brandLanguageCodeMapper
                .ExternalToInternalAsync(currentCultureInfo.Name, cachedPlayer.BrandId)
                .ConfigureAwait(false);

            var subGp = await getGpCodeService
                .GetAsync(new GetGpCodeQuery(brandId, request.SubGameProviderCode))
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

            if (subGp.IsTransferWallet)
                await fundService
                    .ExecuteAsync(new FundOutBeforeEnterGameCommand(
                        cachedPlayer.BrandId,
                        playerTokenData.PlayerCode,
                        subGp.Code))
                    .ConfigureAwait(false);

            return new GetGameUrlResponse
            {
                Error = info.Error,
                ErrorMessage = info.ErrorMessage,
                Url = info.Url
            };
        }

        /// <summary>
        /// get game launch url for lobby type game.
        /// </summary>
        [HttpPost("lobby/geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<GetGameUrlResponse> GetLobbyGameUrlForPostAsync(
            [FromServices] IPlayerOperations playerOperations,
            [FromServices] IStatusValidator playerStatusValidator,
            [FromServices] IBrandLanguageCodeMapper brandLanguageCodeMapper,
            [FromServices] IGetService<GetGpCodeQuery, BrandSubGameProviderInfo> getGpCodeService,
            [FromServices] IGetService<GetLobbyGameUrlQuery, LobbyGameUrlInfo> getLobbyGameUrlQueryService,
            [FromServices] ICommandService<FundOutBeforeEnterGameCommand> fundService,
            [FromBody] GetLobbyGameUrlRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var cachedPlayer = await playerOperations
                .GetCachedPlayerAsync(playerTokenData.PlayerCode)
                .ConfigureAwait(false);

            playerStatusValidator.ThrowIfPlayerDisabled(cachedPlayer.Status);

            var subGp = await getGpCodeService
                .GetAsync(new GetGpCodeQuery(brandId, request.SubGameProviderCode))
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

            if (subGp.IsTransferWallet)
                await fundService
                    .ExecuteAsync(new FundOutBeforeEnterGameCommand(
                        cachedPlayer.BrandId,
                        playerTokenData.PlayerCode,
                        subGp.Code))
                    .ConfigureAwait(false);

            return new GetGameUrlResponse
            {
                Error = info.Error,
                ErrorMessage = info.ErrorMessage,
                Url = info.Url
            };
        }

        [HttpPut("favorite/{gameId}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async ValueTask SetGameToFavoriteAsync(
            [FromServices] ICommandService<SetFavoriteGameCommand> setService,
            [FromServices] IGetService<CountPlayerFavoriteGameQuery, int> countService,
            int gameId)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var count = await countService
                .GetAsync(new CountPlayerFavoriteGameQuery(
                    brandId,
                    playerTokenData.PlayerCode))
                .ConfigureAwait(false);

            if (count >= 100)
                throw new FavoriteCountFullException();

            var command = new SetFavoriteGameCommand(
                brandId,
                playerTokenData.PlayerCode,
                gameId);

            await setService
                .ExecuteAsync(command)
                .ConfigureAwait(false);
        }

        [HttpDelete("favorite/{gameId}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public ValueTask RemoveGameFromFavoriteAsync(
            [FromServices] ICommandService<RemoveFavoriteGameCommand> removeService,
            int gameId)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var command = new RemoveFavoriteGameCommand(
                playerTokenData.PlayerCode,
                gameId);

            return removeService.ExecuteAsync(command);
        }

        [HttpGet("favorite/{gameType}")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public IAsyncEnumerable<int> GetFavoriteGamesAsync(
            [FromServices] IQueryService<ListFavoriteGameIdsQuery, FavoriteGameInfo> listService,
            GameType gameType)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();
            var brandId = HttpContext.Items.GetBrandId();

            var query = new ListFavoriteGameIdsQuery(
                brandId,
                playerTokenData.PlayerCode,
                gameType);

            return listService.QueryAsync(query)
                .Select(fg => fg.GameId);
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
