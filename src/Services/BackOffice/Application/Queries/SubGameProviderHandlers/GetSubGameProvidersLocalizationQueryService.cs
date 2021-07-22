using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Tgs;
using TGS.Services.BackOffice.Application.Models.SubGameProvider;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Queries.SubGameProviderHandlers
{
    internal class
        GetSubGameProvidersLocalizationQueryService : IGetService<GetSubGameProvidersLocalizationRequest,
            SubGameProvidersLocalizationInfo>
    {
        private readonly ITgsRepository _tgsRepository;

        public GetSubGameProvidersLocalizationQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<SubGameProvidersLocalizationInfo> GetAsync(
            GetSubGameProvidersLocalizationRequest query,
            CancellationToken cancellationToken = default)
        {
            var languages = await _tgsRepository.GameProviderLanguages.AsQueryable()
                .Select(x => x.CultureCode).Distinct()
                .ToArrayAsync(cancellationToken);

            var subGameProviders = await _tgsRepository.SubGameProviders.AsQueryable().ToArrayAsync(cancellationToken);

            return new SubGameProvidersLocalizationInfo
            {
                Languages = languages,
                GameProviders = subGameProviders.Select(x =>
                    new SubGameProviderData
                    {
                        SubGameProviderCode = x.Code,
                        SubGameProviderName = x.Name,
                        Localization = x.Localization
                    })
            };
        }
    }
}