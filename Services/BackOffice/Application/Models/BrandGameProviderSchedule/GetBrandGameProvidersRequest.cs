using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.BrandGameProviderSchedule
{
    public class GetBrandGameProvidersRequest : PageableRequest, IRequest<PagedResponse<BrandGameProviderModel>>
    {
        public string Keyword { get; set; }
    }
}