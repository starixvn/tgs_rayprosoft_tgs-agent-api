using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Core.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.TgsApi.GameProviders.Evolution.Controllers
{
    [FeatureGate(Consts.GameProvider.Evolution)]
    [Route("api")]
    [ApiController]
    public class EvolutionGameController : ControllerBase
    {
        [Route("bet")]
        public JsonResult PlaceBetAsync()
        { 
            var obj = new { Error = TgsApiError.None };
            return new JsonResult(obj) { StatusCode = (int) HttpStatusCode.OK };
        }
    }
}
