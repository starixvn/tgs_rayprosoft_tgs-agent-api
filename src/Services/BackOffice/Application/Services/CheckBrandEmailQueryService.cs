using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.Services.BackOffice.Domain;
using TGS.ServiceUtility;

namespace TGS.Services.BackOffice.Application.Services
{
    internal class CheckBrandEmailQueryService : IGetService<CheckBrandEmailQuery, bool>
    {
        private readonly IBackOfficeRepository _backOfficeRepository;

        public CheckBrandEmailQueryService(IBackOfficeRepository backOfficeRepository)
        {
            _backOfficeRepository = backOfficeRepository ?? throw new ArgumentNullException(nameof(backOfficeRepository));
        }

        public ValueTask<bool> GetAsync(CheckBrandEmailQuery query, CancellationToken cancellationToken = default)
        {
            return _backOfficeRepository.CheckBrandEmailAsync(query.BrandId, query.Email);
        }
    }
}
