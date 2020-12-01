using System;
using System.ComponentModel;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Brand;

namespace TGS.Apis.BackendOffice.Controllers.Brand
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class BrandController : BaseContextController
    {
        private readonly IMediator _mediator;

        public BrandController(IMediator mediator,
            IHttpContextAccessor httpContextAccessor) : base(
            httpContextAccessor)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Description("Get brands: 營運商列表")]
        public Task<PagedResponse<BrandModel>> GetBrandsAsync([FromQuery] GetBrandsRequest request)
        {
            return _mediator.Send(request);
        }

        [HttpGet("{id}")]
        [Description("Get a Brand")]
        public Task<GetBrandResponse> GetBrandAsync(Guid id)
        {
            return _mediator.Send(new GetBrandRequest(id));
        }

        [HttpPost]
        [Description("Create a Brand")]
        public Task<MessageResponse> CreateBrandAsync(CreateBrandRequest request)
        {
            return _mediator.Send(request);
        }

        [HttpPut("{id}")]
        [Description("Update a Brand")]
        public Task<MessageResponse> UpdateBrandAsync(Guid id, UpdateBrandRequest request)
        {
            request.Id = id;
            return _mediator.Send(request);
        }
    }
}