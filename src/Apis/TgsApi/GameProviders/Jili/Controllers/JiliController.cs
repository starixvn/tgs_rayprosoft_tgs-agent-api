using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TGS.Apis.TgsApi.GameProviders.Jili.Controllers
{
    [Route("api/jili")]
    [ApiController]
    public class JiliController : ControllerBase
    {
        public ValueTask AuthorizePlayerAsync()
        {
            return default;
        }
    }
}
