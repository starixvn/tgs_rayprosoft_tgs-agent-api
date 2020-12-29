using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Services.BackOffice.Application.Models.Brand;
using TGS.Services.BackOffice.Application.Resources;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.BrandHandlers
{
    public class GetBrandQueryHandler : Handler<GetBrandRequest, GetBrandResponse>
    {
        private readonly IBrandService _service;

        public GetBrandQueryHandler(IBrandService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<GetBrandResponse> Handle(GetBrandRequest request,
            CancellationToken cancellationToken)
        {
            var brand = await _service.GetBrandAsync(x => x.Id == request.Id,
                x => x
                    .Where(ResellerId != null, r => r.Licensee.ResellerId == ResellerId)
                    .Where(LicenseeId != null, l => l.LicenseeId == LicenseeId)
                    .Where(BrandId != null, l => l.Id == BrandId));

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
                    BrandGames = brand.Games.Select(x => new BrandGameModel
                    {
                        GameId = x.GameId
                    }).ToArray(),
                    BetLimits = brand.BetLimits.Select(x => new BrandBetLimitModel
                    {
                        Id = x.BetLimitId,
                        IsDefault = x.IsDefault
                    }).ToArray(),
                    BrandGroups = brand.BrandGroups.Select(x => new BrandGroupModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Code = x.Code,
                        Description = x.Description,
                        IsSports = x.IsSports,
                        BrandGameIds = x.BrandGames.Select(g => g.BrandGame.GameId).ToArray(),
                        GameProviderBetLimits = x.BrandGroupBetLimits.SelectMany(bgb => bgb.GameProviderBetLimits)
                            .Select(b => new BrandGroupGameProviderBetLimitModel
                            {
                                BrandBetLimitId = b.BrandGroupBetLimit.BetLimitId,
                                GameProviderBetLimitId = b.GameProviderBetLimitId
                            }).ToArray()
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