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

        public ValueTask<bool> GetAsync(CheckBrandEmailQuery query, CancellationToken cancellationToken = default)
        {
            var result = _tgsRepository.Players
                .AsQueryable()
                .Any(p => p.BrandId == query.BrandId && p.Email.ToLower() == query.Email.ToLower());

            return ValueTask.FromResult(result);
        }
    }
}
