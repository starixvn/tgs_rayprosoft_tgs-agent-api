using System.ComponentModel;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Models.Messages;

namespace TGS.Apis.OperatorApi.Controllers
{
    public partial class PlayerController
    {
        /// <summary>
        ///     Get player round report
        /// </summary>
        [HttpGet("report/rounds")]
        [Description("Get rounds")]
        [Consumes(MediaTypeNames.Application.Json)]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public Task<PagedResponse<PlayerRoundReportModel>> GetRoundsAsync(
            [FromQuery] GetPlayerRoundReportRequest request) =>
            _mediator.Send(request);
    }
}