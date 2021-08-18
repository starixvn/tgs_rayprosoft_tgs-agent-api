using System;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages.Banner;

namespace TGS.Apis.BackendOffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BannerController : BaseContextController
    {
        [HttpPut("{brandId}")]
        public void CreateBanner(
            Guid brandId,
            [FromBody] BannerModifyRequest request)
        {
        }
    }
}
