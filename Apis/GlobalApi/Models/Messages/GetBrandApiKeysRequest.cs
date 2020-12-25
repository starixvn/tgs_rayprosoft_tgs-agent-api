using MediatR;
using TGS.Apis.GlobalApi.Models.Data;

namespace TGS.Apis.GlobalApi.Models.Messages
{
    public record GetBrandApiKeysRequest : PageableRequest, IRequest<PagedResponse<BrandApiKeyResult>>
    {
    }
}