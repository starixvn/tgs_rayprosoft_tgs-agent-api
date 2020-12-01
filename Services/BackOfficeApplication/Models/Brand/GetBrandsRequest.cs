using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOfficeApplication.Models.Brand
{
    public class GetBrandsRequest : PageableRequest, IRequest<PagedResponse<BrandModel>>
    {
        public string Keyword { get; set; }
    }
}