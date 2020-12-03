using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Brand
{
    public class CreateBrandRequest : BrandRequest, IRequest<MessageResponse>
    {
    }
}