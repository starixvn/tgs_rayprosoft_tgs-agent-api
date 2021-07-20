using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Announcement;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class AnnouncementController : ControllerBase
    {
        [HttpGet("announcement/list")]
        public IAsyncEnumerable<Announcement> GetAnnouncementsAsync(
            [FromServices] IQueryService<GetAnnouncementQuery, AnnouncementInfo> getAnnouncementQueryService)
        {
            var brandId = HttpContext.Items.GetBrandId();

            var query = new GetAnnouncementQuery(brandId);

            return getAnnouncementQueryService.QueryAsync(query)
                .Select(item => new Announcement
                {
                    Content = item.Content,
                    From = item.From,
                    To = item.To,
                    Subject = item.Subject
                });
        }
    }
}
