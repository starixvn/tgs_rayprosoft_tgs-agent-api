using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using TGS.Core.DataServices.Tgs;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.BrandLanguage
{
    internal class GetBrandLanguageQueryService : IQueryService<BrandLanguageQuery, BrandLanguageInfo>
    {
        private readonly ITgsRepository _tgsRepository;

        public GetBrandLanguageQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async IAsyncEnumerable<BrandLanguageInfo> QueryAsync(
            BrandLanguageQuery query,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            var brand = await _tgsRepository.Brands.FirstAsync(x => x.Id == query.BrandId);

            foreach (var language in brand.Languages)
            {
                yield return new BrandLanguageInfo()
                {
                    CultureCode = language.CultureCode
                };
            }
        }
    }
}