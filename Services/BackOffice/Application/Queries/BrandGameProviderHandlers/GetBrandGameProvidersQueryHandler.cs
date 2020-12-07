using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.DataService;
using TGS.Services.BackOffice.Application.Models.BrandGameProvider;

namespace TGS.Services.BackOffice.Application.Queries.BrandGameProviderHandlers
{
    public class
        GetBrandGameProvidersQueryHandler : Handler<GetBrandGameProvidersRequest, PagedResponse<BrandGameProviderModel>>
    {
        private readonly IBrandService _service;

        public GetBrandGameProvidersQueryHandler(IBrandService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override async Task<PagedResponse<BrandGameProviderModel>> Handle(GetBrandGameProvidersRequest request,
            CancellationToken cancellationToken)
        {
            return new();
        }
    }
}