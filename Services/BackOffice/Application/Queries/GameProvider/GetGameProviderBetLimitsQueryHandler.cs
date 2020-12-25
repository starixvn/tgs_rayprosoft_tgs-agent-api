using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Services.BackOffice.Application.Models.GameProvider;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.GameProvider
{
    public class
        GetGameProviderBetLimitsQueryHandler : Handler<GetGameProviderBetLimitsRequest,
            GetGameProviderBetLimitsResponse>
    {
        private readonly IGameProviderBetLimitService _service;

        public GetGameProviderBetLimitsQueryHandler(IGameProviderBetLimitService service, IHttpContextAccessor accessor)
            :
            base(accessor)
        {
            _service = service;
        }

        public override async Task<GetGameProviderBetLimitsResponse> Handle(GetGameProviderBetLimitsRequest request,
            CancellationToken cancellationToken)
        {
            var betLimits = await _service.GetGameProviderBetLimitsAsync()
                .Where(ResellerId != null,
                    r => r.GameProvider.Resellers.Select(x => x.ResellerId).Contains(ResellerId.Value))
                .Where(LicenseeId != null,
                    l => l.GameProvider.Licensees.Select(x => x.LicenseeId).Contains(LicenseeId.Value))
                .Where(BrandId != null, b => b.GameProvider.Brands.Select(x => x.BrandId).Contains(BrandId.Value))
                .Where(x => x.GameProviderId == request.GameProviderId)
                .Select(x => new GameProviderBetLimitModel
                {
                    Id = x.Id,
                    CurrencyCode = x.CurrencyCode,
                    MinBet = x.MinBet,
                    MaxBet = x.MaxBet,
                    MaxBetPerMatch = x.MaxBetPerMatch,
                    ExternalSetting = x.ExternalSetting
                }).ToArrayAsync(cancellationToken);

            return new GetGameProviderBetLimitsResponse
            {
                Valid = true,
                BetLimits = betLimits
            };
        }
    }
}