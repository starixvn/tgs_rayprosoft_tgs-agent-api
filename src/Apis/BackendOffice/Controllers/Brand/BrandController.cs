using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TGS.Apis.BackendOffice.Controllers.Messages;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Brand;
using TGS.Services.BackOffice.Application.Services;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Apis.BackendOffice.Controllers.Brand
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class BrandController : BaseContextController
    {
        private readonly IMediator _mediator;

        public BrandController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Description("Create a Brand")]
        public Task<MessageResponse> CreateBrandAsync(CreateBrandRequest request)
            => _mediator.Send(request);

        [HttpGet("{id}")]
        [Description("Get a Brand")]
        public Task<GetBrandResponse> GetBrandAsync(Guid id)
            => _mediator.Send(new GetBrandRequest(id));

        [HttpGet("logo/{brandId}")]
        public async ValueTask<BrandLogoInfoViewModel> GetBrandLogoInfoAsync(
            [FromServices] IGetService<GetBrandLogoQuery, BrandLogoInfo> getService,
            Guid brandId)
        {
            var query = new GetBrandLogoQuery(brandId);

            var info = await getService
                .GetAsync(query, HttpContext.RequestAborted)
                .ConfigureAwait(false);

            if (info == null)
                return new BrandLogoInfoViewModel
                {
                    BrandId = brandId
                };

            return new BrandLogoInfoViewModel
            {
                BrandId = info.BrandId,
                DesktopLogoUrl = info.DesktopLogoUrl,
                LoadingIconUrl = info.LoadingUrl,
                MobileLogoUrl = info.MobileLogoUrl
            };
        }

        [HttpGet]
        [Description("Get brands: 品牌商列表")]
        public Task<PagedResponse<BrandModel>> GetBrandsAsync([FromQuery] GetBrandsRequest request)
            => _mediator.Send(request);

        [HttpGet("{id}/syncGames")]
        [Description("sync brand games")]
        public Task<MessageResponse> SyncBrandGamesAsync(Guid id)
            => _mediator.Send(new SyncGamesRequest(id));

        [HttpPut("{id}")]
        [Description("Update a Brand")]
        public Task<MessageResponse> UpdateBrandAsync(Guid id, UpdateBrandRequest request)
        {
            request.Id = id;
            return _mediator.Send(request);
        }
    }
}
