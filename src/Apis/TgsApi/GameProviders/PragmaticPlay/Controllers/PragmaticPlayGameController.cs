using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Core.Models;
using TGS.Shared.Game.Models.PragmaticPlay.Incoming;

namespace TGS.Apis.TgsApi.GameProviders.PragmaticPlay.Controllers
{
    [Route("api/provider")]
    [ApiController]
    [Consumes("application/x-www-form-urlencoded")]
    [Produces(MediaTypeNames.Application.Json)]
    [AllowAnonymous]
    [FeatureGate(Consts.GameProvider.PragmaticPlay)]
    public sealed class PragmaticPlayGameController : ControllerBase
    {
        [HttpPost]
        [Route("authenticate")]
        public ValueTask<AuthorizePlayerResponse> AuthorizePlayerAsync(AuthorizePlayerRequest request) => new();
    }
}