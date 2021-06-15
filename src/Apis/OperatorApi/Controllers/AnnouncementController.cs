using System.Net.Mime;
using System.Threading.Tasks;
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
        public async Task<GetAnnouncementsResponse> GetAnnouncementsAsync(
            [FromServices] IGetService<GetAnnouncementQuery, AnnouncementInfo> getAnnouncementQueryService)
        {
            var brandId = HttpContext.Items.GetBrandId();

            var query = new GetAnnouncementQuery(brandId);

            var info = await getAnnouncementQueryService.GetAsync(query).ConfigureAwait(false);

            return new GetAnnouncementsResponse()
            {
                Announcements = info.Announcements
            };
        }
    }
}