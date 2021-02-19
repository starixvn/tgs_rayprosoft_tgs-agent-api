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
                    .Where(ResellerId, r => r.Licensee.ResellerId == ResellerId)
                    .Where(LicenseeId, l => l.LicenseeId == LicenseeId)
                    .Where(BrandId, l => l.Id == BrandId));

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
                    LicenseeId = brand.LicenseeId,
                    LicenseeName = brand.Licensee.Name,
                    Code = brand.Code,
                    Name = brand.Name,
                    Status = brand.Status,
                    ClientId = brand.ClientId,
                    SecretKey = brand.SecretKey,
                    ApiKey = brand.ApiKey,
                    Currencies = brand.Currencies
                        .Select(x => new BrandCurrencyModel
                        {
                            Code = x.CurrencyCode,
                            Name = x.Currency.Name
                        }).ToArray(),
                    Languages = brand.Languages
                        .Select(x => new BrandLanguageModel
                        {
                            CultureCode = x.CultureCode,
                            BrandCultureCode = x.BrandCultureCode
                        }).ToArray(),
                    GameProviders = brand.GameProviders
                        .Select(x => new BrandGameProviderModel
                        {
                            Id = x.Id,
                            GameProviderId = x.GameProviderId,
                            Name = x.GameProvider.Name,
                            ConfigurationId = x.GameProviderConfigurationId,
                            DefaultLanguageCode = x.DefaultLanguageCode,
                            DesktopQueryString = x.DesktopQueryString,
                            MobileQueryString = x.MobileQueryString
                        }).ToArray(),
                    Games = brand.Games.Select(x => new BrandGameModel
                    {
                        GameProviderId = x.Game.GameProviderId,
                        GameId = x.GameId,
                        GameName = x.Game.Name,
                        GameType = x.Game.Type
                    }).ToArray(),
                    BetLimits = brand.BetLimits.Select(x => new BrandBetLimitModel
                    {
                        Id = x.BetLimitId,
                        CurrencyCode = x.BetLimit.CurrencyCode,
                        IsDefault = x.IsDefault
                    }).ToArray(),
                    BrandGroups = brand.BrandGroups.Select(x => new BrandGroupModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        IsSports = x.IsSports,
                        BrandGames = x.BrandGames.Select(g => new BrandGameModel
                        {
                            GameProviderId = g.BrandGame.Game.GameProviderId,
                            GameId = g.BrandGame.GameId,
                            GameName = g.BrandGame.Game.Name,
                            GameType = g.BrandGame.Game.Type
                        }).ToArray(),
                        GameProviderBetLimits = x.BrandGroupBetLimits.SelectMany(bgb => bgb.GameProviderBetLimits)
                            .Select(b => new BrandGroupGameProviderBetLimitModel
                            {
                                GameProviderId = b.GameProviderBetLimit.GameProviderId,
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