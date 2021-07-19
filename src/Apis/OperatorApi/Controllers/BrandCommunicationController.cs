using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
using TGS.Apis.OperatorApi.Extensions;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Services.Operator;
using TGS.Services.Operator.Services.BrandCommunication;
using TGS.ServiceUtility;

namespace TGS.Apis.OperatorApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandCommunicationController : ControllerBase
    {
        [HttpGet]
        [Description("Get a brand's communications")]
        [AuthorizePlayerToken(AllowNullOrEmpty = false, NeedAuthorize = true)]
        [TgsResponseWrapper]
        public IAsyncEnumerable<BrandCommunicationModel> GetBrandCommunicationsAsync(
            [FromServices] IQueryService<BrandCommunicationQuery, BrandCommunicationInfo> queryService)
        {
            var brandId = HttpContext.Items.GetBrandId();

            var query = new BrandCommunicationQuery(brandId);
            var info = queryService.QueryAsync(query);

            return info.Select(x =>
                new BrandCommunicationModel()
                {
                    Id = x.Id.ToString(),
                    Type = x.CommunicationType,
                    Phone = x.Phone,
                    AppId = x.AppId,
                    QRCodeUrl = x.QRCodeUrl
                });
        }
    }
}