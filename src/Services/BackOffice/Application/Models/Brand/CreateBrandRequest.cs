using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Brand
{
    public class CreateBrandRequest : BrandRequest, IRequest<MessageResponse>
    {
        public AgentDividendType AgentDividendType { get; set; }
    }
}