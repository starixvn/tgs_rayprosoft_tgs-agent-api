using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator;
using TGS.Services.Operator.StationLatters;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/latter")]
    [ApiController]
    public class StationLatterController : ControllerBase
    {
        [HttpGet]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public IAsyncEnumerable<StationLatterResponse> GetLattersAsync(
            [FromServices] IQueryService<ListStationLattersQuery, StationLatter> listStationLatterService)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var query = new ListStationLattersQuery(playerTokenData.PlayerCode);

            return listStationLatterService.QueryAsync(query)
                .Select(latter => new StationLatterResponse
                {
                    Subject = latter.Subject,
                    Content = latter.Content,
                    CreateOn = latter.CreateOn
                });
        }
    }
}
