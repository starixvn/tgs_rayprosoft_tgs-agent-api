using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tgs.ServiceUtility;
using TGS.Apis.BackendOffice.Controllers.Messages.BrandToken;
using TGS.Services.BackOffice.Application.Queries.BrandToken;

namespace TGS.Apis.BackendOffice.Controllers.Brand
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandTokenController : BaseContextController
    {
        [HttpGet("{brandId}")]
        [Description("Get a brand's token")]
        public async Task<GetBrandTokenResponse> GetBrandTokenAsync(
            Guid brandId,
            [FromServices] IGetService<BrandTokenQuery, string> getService)
        {
            var query = new BrandTokenQuery(brandId);
            var info = await getService
                .GetAsync(query)
                .ConfigureAwait(false);

            if (info == null)
                return new GetBrandTokenResponse();

            return new GetBrandTokenResponse()
            {
                BrandToken = new BrandTokenModel
                {
                    Token = info
                }
            };
        }
    }
}
