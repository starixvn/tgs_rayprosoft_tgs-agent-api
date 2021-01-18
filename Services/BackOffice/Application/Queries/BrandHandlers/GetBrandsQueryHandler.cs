using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.Models.Brand;
using TGS.Services.BackOffice.Infrastructure.DataService;

namespace TGS.Services.BackOffice.Application.Queries.BrandHandlers
{
    public class GetBrandsQueryHandler : Handler<GetBrandsRequest, PagedResponse<BrandModel>>
    {
        private readonly IBrandService _service;

        public GetBrandsQueryHandler(IBrandService service, IHttpContextAccessor accessor) : base(accessor)
        {
            _service = service;
        }

        public override Task<PagedResponse<BrandModel>> Handle(GetBrandsRequest request,
            CancellationToken cancellationToken)
        {
            return _service.GetBrandsAsync()
                .Where(request.BrandId, x => x.Id == request.BrandId)
                .Where(ResellerId != null, r => r.Licensee.ResellerId == ResellerId)
                .Where(LicenseeId != null, l => l.LicenseeId == LicenseeId)
                .Where(BrandId != null, l => l.Id == BrandId)
                .Where(request.Keyword, x => x.Code.Contains(request.Keyword) || x.Name.Contains(request.Keyword) ||
                                             x.Description.Contains(request.Keyword) ||
                                             x.OperationalEmail.Contains(request.Keyword))
                .Select(x => new BrandModel
                {
                    Id = x.Id,
                    LicenseeId = x.LicenseeId,
                    Name = x.Name,
                    Code = x.Code,
                    Status = x.Status,
                    Description = x.Description,
                    OperationalEmail = x.OperationalEmail,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn
                }).CreatePagedResponseAsync(request);
        }
    }
}