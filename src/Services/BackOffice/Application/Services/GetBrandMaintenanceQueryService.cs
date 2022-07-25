using System;
using System.Threading;
using System.Threading.Tasks;
using Tgs.ServiceUtility;
using TGS.Services.BackOffice.Domain;

namespace TGS.Services.BackOffice.Application.Services
{
    internal class GetBrandMaintenanceQueryService : IGetService<GetBrandMaintenanceQuery, BrandMaintenanceModel>
    {
        private readonly IBackOfficeRepository _backOfficeRepository;

        public GetBrandMaintenanceQueryService(IBackOfficeRepository backOfficeRepository)
        {
            _backOfficeRepository = backOfficeRepository ?? throw new ArgumentNullException(nameof(backOfficeRepository));
        }

        public ValueTask<BrandMaintenanceModel> GetAsync(GetBrandMaintenanceQuery query, CancellationToken cancellationToken = default)
            => _backOfficeRepository.GetBrandMaintenanceByBrandCodeAsync(query.Id);
    }
}
