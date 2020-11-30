using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TGS.Apis.BackendOffice.Controllers.Messages.Brand;
using TGS.Apis.BackendOffice.DataService.Services;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;

namespace TGS.Apis.BackendOffice.Controllers.Brand
{
    [Route("api/[controller]")]
    [ApiController]
    public partial class BrandController : BaseContextController
    {
        private readonly IBrandService _service;

        public BrandController(IBrandService service, IHttpContextAccessor httpContextAccessor) : base(
            httpContextAccessor)
        {
            _service = service;
        }

        [HttpGet]
        [Description("Get brands: 營運商列表")]
        public Task<PagedResponse<BrandModel>> GetBrandsAsync([FromQuery] GetBrandsRequest request)
        {
            return _service.GetBrandsAsync().Where(request.Keyword,
                    x => x.Code.Contains(request.Keyword) || x.Name.Contains(request.Keyword) ||
                         x.Description.Contains(request.Keyword) || x.OperationalEmail.Contains(request.Keyword))
                .Select(x => new BrandModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    Status = x.Status,
                    Description = x.Description,
                    OperationalEmail = x.OperationalEmail,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn
                })
                .AsNoTracking().CreatePagedResponseAsync(request);
        }

        [HttpGet("{id}")]
        [Description("Get a Brand")]
        public async Task<GetBrandResponse> GetBrandAsync(Guid id)
        {
            var brand = await _service.GetBrandAsync(x => x.Id == id,
                x => x.Include(i => i.Currencies)
                    .Include(i => i.Languages)
                    .Include(i => i.GameProviders), true);

            if (brand == null)
            {
                return CreateInvalidResponse<GetBrandResponse>(Resources.Brand.NotFound);
            }

            return new GetBrandResponse
            {
                Valid = true,
                Brand = new BrandModel
                {
                    Id = brand.Id,
                    Code = brand.Code,
                    Name = brand.Name,
                    Status = brand.Status,
                    Currencies = brand.Currencies
                        .Select(x => new BrandCurrencyModel
                        {
                            Code = x.CurrencyCode,
                            Name = x.Currency.Name
                        }).ToArray(),
                    Languages = brand.Languages
                        .Select(x => new BrandLanguageModel
                        {
                            CultureCode = x.CultureCode
                        }).ToArray(),
                    GameProviders = brand.GameProviders
                        .Select(x => new BrandGameProviderModel
                        {
                            Id = x.GameProviderId,
                            Name = x.GameProvider.Name
                        }).ToArray(),

                    Description = brand.Description,
                    OperationalEmail = brand.OperationalEmail,
                    CreatedOn = brand.CreatedOn,
                    UpdatedOn = brand.UpdatedOn
                }
            };
        }

        [HttpPost]
        [Description("Create a Brand")]
        public async Task<MessageResponse> CreateBrandAsync(CreateBrandRequest request)
        {
            await _service.CreateBrandAsync(request, Username);

            return CreateValidResponse<MessageResponse>();
        }

        [HttpPut("{id}")]
        [Description("Update a Brand")]
        public async Task<MessageResponse> UpdateBrandAsync(Guid id, UpdateBrandRequest request)
        {
            var brand = await _service.GetBrandAsync(q => q.Id == id, tracking: true);

            if (brand == null)
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Brand.NotFound);
            }

            await _service.UpdateBrandAsync(brand, request, Username);

            return CreateValidResponse<MessageResponse>();
        }
    }
}