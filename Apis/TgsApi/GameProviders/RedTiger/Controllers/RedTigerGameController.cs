using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Core.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.TgsApi.GameProviders.RedTiger.Controllers
{
    [FeatureGate(Consts.GameProvider.RedTiger)]
    [Route("api")]
    [ApiController]
    public class RedTigerGameController : ControllerBase
    {
        [Route("bet/places")]
        public JsonResult PlaceBetAsync()
        {
            var obj = new { Error = TgsApiError.None };
            return new JsonResult(obj) { StatusCode = (int)HttpStatusCode.OK };
        }
    }
}
