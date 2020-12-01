using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Services.BackOfficeApplication.DataService.Services;
using TGS.Services.BackOfficeApplication.Models.Brand;
using TGS.Services.BackOfficeApplication.Resources;

namespace TGS.Services.BackOfficeApplication.Queries.BrandHandlers
{
    public class GetAllCarsQueryHandler : Handler<GetBrandRequest, GetBrandResponse>
    {
        private readonly IBrandService _service;

        public GetAllCarsQueryHandler(IBrandService service)
        {
            _service = service;
        }

        public override async Task<GetBrandResponse> Handle(GetBrandRequest request,
            CancellationToken cancellationToken)
        {
            var brand = await _service.GetBrandAsync(x => x.Id == request.Id,
                x => x.Include(i => i.Currencies)
                    .Include(i => i.Languages)
                    .Include(i => i.GameProviders), true);

            if (brand == null)
            {
                return CreateInvalidResponse<GetBrandResponse>(Brand.NotFound);
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
    }
}