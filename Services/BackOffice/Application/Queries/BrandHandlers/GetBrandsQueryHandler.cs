using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOffice.Application.DataService;
using TGS.Services.BackOffice.Application.Models.Brand;

namespace TGS.Services.BackOffice.Application.Queries.BrandHandlers
{
    public class GetBrandsQueryHandler : Handler<GetBrandsRequest, PagedResponse<BrandModel>>
    {
        private readonly IBrandService _service;

        public GetBrandsQueryHandler(IBrandService service, IHttpContextAccessor accessor)
        {
            _service = service;
        }

        public override Task<PagedResponse<BrandModel>> Handle(GetBrandsRequest request,
            CancellationToken cancellationToken)
        {
            return _service.GetBrandsAsync()
                .Where(ResellerId != null, r => r.Licensee.ResellerId == ResellerId)
                .Where(LicenseeId != null, l => l.LicenseeId == LicenseeId)
                .Where(BrandId != null, l => l.Id == BrandId)
                .Where(request.Keyword, x => x.Code.Contains(request.Keyword) || x.Name.Contains(request.Keyword) ||
                                             x.Description.Contains(request.Keyword) ||
                                             x.OperationalEmail.Contains(request.Keyword))
                .Select(x => new BrandModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Code = x.Code,
                    Status = x.Status,
                    Description = x.Description,
                    OperationalEmail = x.OperationalEmail,
                    CreatedOn = x.CreatedOn,
                    UpdatedOn = x.UpdatedOn
                })
                .AsNoTracking().CreatePagedResponseAsync(request);
        }
    }
}