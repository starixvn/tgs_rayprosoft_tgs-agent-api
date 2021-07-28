using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models.Enums;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Game;
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
        public async Task<GetGamesResponse> GetGamesAsync(
            [FromServices] IQueryService<GetGamesQuery, GameInfo> getGamesQueryService,
            [FromQuery] GetGamesRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var query = new GetGamesQuery(
                brandId,
                request.CurrencyCode,
                request.GameProviderCode,
                playerTokenData,
                request.PlatformType,
                request.Types ?? Array.Empty<GameType>(),
                request.LanguageCode);

            var infos = await getGamesQueryService
                .QueryAsync(query)
                .OrderBy(g => g.DisplayOrder)
                .ThenBy(g => g.Code)
                .ToListAsync()
                .ConfigureAwait(false);

            var games = infos.Select(info =>
                    new GameModel
                    {
                        Code = info.Code,
                        GameProviderCode = info.GameProviderCode,
                        Description = info.Description,
                        DisplayOrder = info.DisplayOrder,
                        Id = info.Id,
                        IsActive = info.IsActive,
                        Name = info.Name,
                        Tag = new Tag
                        {
                            Hot = info.Tag.Hot,
                            New = info.Tag.New,
                            Recommend = info.Tag.Recommend
                        },
                        Type = info.Type
                    })
                .ToArray();

            return new GetGamesResponse {Games = games};
        }

        /// <summary>
        ///     get game launch url via game id
        /// </summary>
        [HttpPost("geturl")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public async Task<GetGameUrlResponse> GetGameUrlForPostAsync(
            [FromServices] IGetService<GetGameUrlQuery, GameUrlInfo> getGameUrlQueryService,
            [FromBody] GetGameUrlRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var query = new GetGameUrlQuery(
                playerTokenData,
                request.GameProviderCode,
                request.GameId,
                request.LanguageCode,
                request.ExtendedData);
            var info = await getGameUrlQueryService
                .GetAsync(query)
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
            [FromServices] IGetService<GetLobbyGameUrlQuery, LobbyGameUrlInfo> getLobbyGameUrlQueryService,
            [FromBody] GetLobbyGameUrlRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var query = new GetLobbyGameUrlQuery(
                request.GameProviderCode,
                playerTokenData,
                request.GameType,
                request.LanguageCode,
                request.Extends);

            var info = await getLobbyGameUrlQueryService
                .GetAsync(query)
                .ConfigureAwait(false);

            return new GetGameUrlResponse
            {
                Error = info.Error,
                ErrorMessage = info.ErrorMessage,
                Url = info.Url
            };
        }
    }
}
