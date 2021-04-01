using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models.Messages;
using TGS.Services.Operator;
using TGS.Services.Operator.StationLatters;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/latter")]
    [ApiController]
    public class StationLatterController : ControllerBase
    {
        /// <summary>
        /// Gets the station latters.
        /// </summary>
        /// <param name="listStationLatterService">The list station latter service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public IAsyncEnumerable<StationLatterResponse> GetLattersAsync(
            [FromServices] IQueryService<ListStationLattersQuery, StationLatter> listStationLatterService,
            [FromQuery] PagingRequest request)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            var paging = new PageableRequest();
            if (request.ShowAll)
                paging.ShowAll = true;
            else
            {
                paging.Page = request.Page;
                paging.Rows = request.Rows;
                paging.OrderBy = request.IsDesc ? SortOrder.Descending : SortOrder.Ascending;
                paging.SortBy = request.SortBy;
            }

            var query = new ListStationLattersQuery(playerTokenData.PlayerCode, paging);

            return listStationLatterService.QueryAsync(query)
                .Select(latter => new StationLatterResponse
                {
                    Id = latter.Id,
                    Subject = latter.Subject,
                    Content = latter.Content,
                    CreateOn = latter.CreateOn
                });
        }

        [HttpPost]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task SetHaveReadAsync(
            [FromServices] ICommandService<SetHaveReadCommand> setHaveReadService,
            [FromBody] IEnumerable<Guid> stationLatterIds)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            foreach (var id in stationLatterIds)
                await setHaveReadService
                    .ExecuteAsync(new SetHaveReadCommand(playerTokenData.PlayerCode, id))
                    .ConfigureAwait(false);
        }
    }
}
