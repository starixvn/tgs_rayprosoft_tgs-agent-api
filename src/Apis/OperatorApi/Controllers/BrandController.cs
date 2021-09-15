using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.Brand;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        [HttpGet]
        [Description("Get a brand")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = false)]
        public ValueTask<BrandInfo> GetBrandAsync(
            [FromServices] IGetService<BrandQuery, BrandInfo> queryService)
        {
            var brandId = HttpContext.Items.GetBrandId();

            return queryService.GetAsync(new BrandQuery(brandId));
        }
    }
}