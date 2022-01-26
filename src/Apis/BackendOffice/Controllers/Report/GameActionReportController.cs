using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Models.GameAction;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using Tgs.Report.Client;
using Tgs.Report.Client.Models.GameAction;

namespace TGS.Apis.BackendOffice.Controllers.Report
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameActionReportController : BaseContextController
    {
        [HttpGet]
        [Description("Get game actions")]
        public async Task<PagedResponse<GameActionReportModel>> GetGameActionsAsync(
            [FromServices] IReportClient client,
            [FromQuery] GetGameActionsReportRequest request)
        {
            var reportRequest = new GetGameActionsRequest
            {
                RoundId = request.RoundId,
                Page = request.Page,
                Rows = request.Rows,
                OrderBy = (short)request.OrderBy,
                SortBy = request.SortBy,
                ShowAll = request.ShowAll
            };

            var response = await client.GetGameActionsAsync(reportRequest);

            return new()
            {
                Valid = response.Valid,
                Message = response.Message,
                TotalCount = response.TotalCount,
                Items = response.Items?.Select(x => new GameActionReportModel
                {
                    RecordedOn = x.RecordedOn,
                    ExternalTransactionId = x.ExternalTransactionId,
                    GameActionType = (GameActionType)x.GameActionType,
                    Amount = x.Amount,
                    BeforeBalance = x.BeforeBalance,
                    AfterBalance = x.AfterBalance
                }).ToArray()
            };
        }
    }
}