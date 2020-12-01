using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOfficeApplication.Models.Brand
{
    public class UpdateBrandRequest : BrandRequest, IRequest<MessageResponse>
    {
    }
}