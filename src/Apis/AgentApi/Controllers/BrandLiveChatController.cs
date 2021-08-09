using System.ComponentModel;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Services.Agent;
using TGS.Services.Agent.Services.BrandLiveChat;
using TGS.ServiceUtility;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class BrandLiveChatController : ControllerBase
    {
        [HttpGet]
        [Description("Get a brand's live chat")]
        [AuthorizeAgentToken(AllowNullOrEmpty = true, NeedAuthorize = false)]
        public async Task<BrandLiveChatModel> GetBrandLiveChatAsync(
          [FromServices] IGetService<BrandLiveChatQuery, BrandLiveChatInfo> queryService)
        {
            var brandId = HttpContext.Items.GetBrandId();

            var query = new BrandLiveChatQuery(brandId);

            var info = await queryService.GetAsync(query).ConfigureAwait(false);

            if (info == null)
                return null;

            return new BrandLiveChatModel()
            {
                Name = info.Name,
                Setting = JsonDocument.Parse(info.Setting)
            };
        }
    }
}