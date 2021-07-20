using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.GameProvider;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class GameProviderController : ControllerBase
    {
        /// <summary>
        ///     Get game provider list
        /// </summary>
        [HttpGet("list")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public async Task<GetGameProvidersResponse> GetGameProvidersAsync(
            [FromServices] IQueryService<GetGameProvidersQuery, GameProviderInfo> getGameProviderQueryService,
            [FromQuery] GetGameProvidersRequest request)
        {
            var brandId = HttpContext.Items.GetBrandId();
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var query = new GetGameProvidersQuery(
                brandId,
                request.CurrencyCode,
                playerTokenData,
                request.PlatformType,
                request.LanguageCode);
            var infos = await getGameProviderQueryService
                .QueryAsync(query)
                .ToListAsync()
                .ConfigureAwait(false);

            var gameProviders = infos.Select(info =>
                    new GameProviderModel
                    {
                        Code = info.Code,
                        Description = info.Description,
                        GameTypes = info.GameTypes,
                        IsTransferWallet = info.IsTransferWallet,
                        Name = info.Name,
                        Status = info.Status
                    })
                .ToArray();

            return new GetGameProvidersResponse {GameProviders = gameProviders};
        }
    }
}