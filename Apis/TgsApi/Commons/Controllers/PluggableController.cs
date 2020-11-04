using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages.Pluggables;

namespace TGS.Apis.TgsApi.Commons.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public sealed class PluggableController : ControllerBase
    {
        [HttpPost, Route("gameurl")]
        public Task<GetGameUrlResponse> GetGameUrlAsync(GetGameUrlRequest request)
        {
            return Task.FromResult(new GetGameUrlResponse
            {
                Url = "http://testgame.com/launch"
            });
        }
    }
}
