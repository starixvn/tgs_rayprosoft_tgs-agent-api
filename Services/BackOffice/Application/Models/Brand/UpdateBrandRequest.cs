using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Brand
{
    public class UpdateBrandRequest : BrandRequest, IRequest<MessageResponse>
    {
        public BrandGroupModel[] BrandGroups { get; set; }
        public BrandGameModel[] Games { get; set; }
    }
}