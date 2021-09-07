using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Player
{
    internal class CheckBrandEmailQueryService : IGetService<CheckBrandEmailQuery, bool>
    {
        private readonly ITgsRepository _tgsRepository;

        public CheckBrandEmailQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<bool> GetAsync(CheckBrandEmailQuery query, CancellationToken cancellationToken = default)
        {
            return await _tgsRepository.Players
                .AsAsyncEnumerable()
                .Where(p => p.BrandId == query.BrandId && !string.IsNullOrEmpty(p.Email))
                .AnyAsync(p => p.BrandId == query.BrandId && p.Email.ToLower() == query.Email.ToLower())
                .ConfigureAwait(false);
        }
    }
}
