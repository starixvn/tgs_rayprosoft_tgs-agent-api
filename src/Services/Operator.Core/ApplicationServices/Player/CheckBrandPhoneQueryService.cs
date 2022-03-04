using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Core.DataServices.Tgs;

namespace TGS.Services.Operator.ApplicationServices.Player
{
    internal class CheckBrandPhoneQueryService : IGetService<CheckBrandPhoneQuery, bool>
    {
        private readonly ITgsRepository _tgsRepository;

        public CheckBrandPhoneQueryService(ITgsRepository tgsRepository)
        {
            _tgsRepository = tgsRepository ?? throw new ArgumentNullException(nameof(tgsRepository));
        }

        public ValueTask<bool> GetAsync(CheckBrandPhoneQuery query, CancellationToken cancellationToken = default)
        {
            var result = _tgsRepository.Players
                .AsQueryable()
                .Any(p => p.BrandId == query.BrandId
                    && p.AreaCode == query.AreaCode
                    && p.Phone == query.Phone
                    && p.PlayerCode != query.PlayerCode);

            return ValueTask.FromResult(result);
        }
    }
}
