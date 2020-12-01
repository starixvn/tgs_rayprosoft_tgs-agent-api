using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOfficeApplication.Models.Brand
{
    public class CreateBrandRequest : BrandRequest, IRequest<MessageResponse>
    {
    }
}