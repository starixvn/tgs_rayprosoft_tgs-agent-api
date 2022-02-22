using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Apis.AgentApi.Attributes;
using TGS.Apis.AgentApi.Extensions;
using TGS.Apis.AgentApi.Models;
using TGS.Services.Agent;
using TGS.Services.Agent.ApplicationServices.GameProvder;

namespace TGS.Apis.AgentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    public class GameProviderController : ControllerBase
    {
        [HttpGet]
        [Description("Get game providers")]
        [AuthorizeAgentToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        public IAsyncEnumerable<GameProviderBaseModel> GetGameProvidersAsync(
            [FromServices] IQueryService<GetGameProvidersQuery, GameProviderInfo> queryService)
        {
            var brandId = HttpContext.Items.GetBrandId();

            return queryService.QueryAsync(new GetGameProvidersQuery(brandId))
                .Select(info => new GameProviderBaseModel
                {
                    Id = info.Id,
                    Name = info.Name
                });
        }
    }
}
