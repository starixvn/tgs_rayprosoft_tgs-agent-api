using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.SBOBET.Filters;
using TGS.Core.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.TgsApi.GameProviders.SBOBET.Controllers
{
    [FeatureGate(Consts.GameProvider.SBOBET)]
    [Route("api")]
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [TypeFilter(typeof(HandleSBOErrorFilter))]
    public class SBOBETGameController : ControllerBase
    {
        [Route("bet")]
        public JsonResult PlaceBetAsync()
        {
            var obj = new { Error = TgsApiError.None };
            return new JsonResult(obj) { StatusCode = (int)HttpStatusCode.OK };
        }
    }
}
