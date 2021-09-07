using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TGS.Core.DataServices.Tgs;
using TGS.ServiceUtility;

namespace TGS.Services.Operator.Services.Player
{
    internal class CheckBrandEmailCommandService : IGetService<CheckBrandEmailCommand, bool>
    {
        private readonly ITgsRepository _tgsRepository;

        public CheckBrandEmailCommandService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public async ValueTask<bool> GetAsync(CheckBrandEmailCommand query, CancellationToken cancellationToken = default)
        {
            return await _tgsRepository.Players
                .AnyAsync(p => p.BrandId == query.BrandId && p.Email.ToLower() == query.Email.ToLower())
                .ConfigureAwait(false);
        }
    }
}
