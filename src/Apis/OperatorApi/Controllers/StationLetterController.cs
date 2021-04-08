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
using TGS.Services.Operator.StationLetters;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/letter")]
    [ApiController]
    public class StationLetterController : ControllerBase
    {
        /// <summary>
        /// Gets the station letters.
        /// </summary>
        /// <param name="listStationLetterService">The list station letter service.</param>
        /// <param name="countStationLetterService">The count station letter service.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task<PagingResponse<StationLetterResponse>> GetLettersAsync(
            [FromServices] IQueryService<GetStationLettersQuery, StationLetter> listStationLetterService,
            [FromServices] IGetService<GetStationLettersQuery, int> countStationLetterService,
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

            var query = new GetStationLettersQuery(playerTokenData.PlayerCode, paging);

            return new PagingResponse<StationLetterResponse>
            {
                TotalCount = await countStationLetterService
                    .GetAsync(query)
                    .ConfigureAwait(false),
                Items = listStationLetterService.QueryAsync(query)
                    .Select(letter => new StationLetterResponse
                    {
                        Id = letter.Id,
                        Subject = letter.Subject,
                        Content = letter.Content,
                        CreateOn = letter.CreateOn,
                        IsRead = letter.IsRead
                    })
                    .ToEnumerable()
            };
        }

        [HttpPost]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public async Task SetHaveReadAsync(
            [FromServices] ICommandService<SetHaveReadCommand> setHaveReadService,
            [FromBody] IEnumerable<Guid> stationLetterIds)
        {
            var playerTokenData = HttpContext.Items.GetPlayerTokenData();

            foreach (var id in stationLetterIds)
                await setHaveReadService
                    .ExecuteAsync(new SetHaveReadCommand(playerTokenData.PlayerCode, id))
                    .ConfigureAwait(false);
        }
    }
}
