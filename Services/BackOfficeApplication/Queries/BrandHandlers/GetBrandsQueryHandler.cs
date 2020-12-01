using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TGS.Core.DataServices.Base.Extensions;
using TGS.Core.Models.Messages;
using TGS.Services.BackOfficeApplication.DataService.Services;
using TGS.Services.BackOfficeApplication.Models.Brand;

namespace TGS.Services.BackOfficeApplication.Queries.BrandHandlers
{
    public class GetBrandsQueryHandler : Handler<GetBrandsRequest, PagedResponse<BrandModel>>
    {
        private readonly IBrandService _service;

        public GetBrandsQueryHandler(IBrandService service)
        {
            _service = service;
        }

        public override Task<PagedResponse<BrandModel>> Handle(GetBrandsRequest request,
            CancellationToken cancellationToken)
        {
            return _service.GetBrandsAsync().Where(request.Keyword,
                    x => x.Code.Contains(request.Keyword) || x.Name.Contains(request.Keyword) ||
                         x.Description.Contains(request.Keyword) || x.OperationalEmail.Contains(request.Keyword))
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