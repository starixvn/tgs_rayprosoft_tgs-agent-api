using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Apis.TgsApi.GameProviders.RedTiger.Filters;
using TGS.Core.Models.Enums;
using static TGS.Core.Models.Consts;

namespace TGS.Apis.TgsApi.GameProviders.RedTiger.Controllers
{
    [FeatureGate(GameProvider.RedTiger)]
    [AllowAnonymous]
    [Route("api")]
    [ApiController] 
    [TypeFilter(typeof(HandleRedTigerErrorFilter))]
    public class RedTigerGameController : ControllerBase
    {
        [HttpPost]
        public IActionResult CommomEntrance() 
        { 
            var obj = new { Error = TgsApiError.None, Action = "CommomEntrance" };
            return new JsonResult(obj) { StatusCode = (int)HttpStatusCode.OK };
        }

        [HttpPost]
        [Route("bet/places")]
        [Consumes("application/json")]
        public JsonResult PlaceBetAsync()
        {
            var obj = new { Error = TgsApiError.None, Action = "bet" };
            return new JsonResult(obj) { StatusCode = (int)HttpStatusCode.OK };
        }

        [HttpPost]
        [Route("bet/error")]
        [Consumes("application/json")]
        public JsonResult ErrorAsync(Model model)
        {
            throw new InvalidOperationException(model.Value.ToString());
        }

        public class Model
        {
            public int Value { get;set; }
        }
    }
}