using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.OperatorApi.Attributes;
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
        [HttpGet("{brandId}")]
        [Description("Get a brand's communications")]
        [AuthorizePlayerToken(AllowNullOrEmpty = true, NeedAuthorize = true)]
        public IAsyncEnumerable<BrandCommunicationModel> GetBrandCommunicationsAsync(
            Guid brandId,
            [FromServices] IQueryService<BrandCommunicationQuery, BrandCommunicationInfo> queryService)
        {
            var query = new BrandCommunicationQuery(brandId);
            var info = queryService.QueryAsync(query);

            return info.Select(x =>
                new BrandCommunicationModel()
                {
                    Type = x.CommunicationType,
                    Phone = x.Phone,
                    AppId = x.AppId,
                    QRCodeUrl = x.QRCodeUrl
                })
                .Take(5);
        }
    }
}