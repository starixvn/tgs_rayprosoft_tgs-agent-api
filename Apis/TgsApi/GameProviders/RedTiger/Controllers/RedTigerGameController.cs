using System;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using TGS.Core.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.TgsApi.GameProviders.RedTiger.Controllers
{
    [FeatureGate(Consts.GameProvider.RedTiger)]
    [AllowAnonymous]
    [Route("api")]
    [ApiController]
    [Consumes("application/json")]
    public class RedTigerGameController : ControllerBase
    {
        [Route("bet/places")]
        public JsonResult PlaceBetAsync()
        {
            var obj = new { Error = TgsApiError.None };
            return new JsonResult(obj) { StatusCode = (int)HttpStatusCode.OK };
        }

        [HttpPost]
        [Route("bet/error")]
        public JsonResult ErrorAsync(Model model)
        {
            throw new InvalidOperationException("111");
        }

        public class Model
        {
            public int Value { get;set; }
        }
    }
}