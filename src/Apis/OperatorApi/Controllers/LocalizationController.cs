using System.Threading;
using Microsoft.AspNetCore.Mvc;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        [HttpGet]
        public string GetLocalizeDictionary()
        {
            return $"{Thread.CurrentThread.CurrentCulture.Name}, {Thread.CurrentThread.CurrentUICulture.Name}";
        }
    }
}
