using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.BackOffice.Application.Models.Brand
{
    public class UpdateBrandStatusRequest : BaseRequest, IRequest<MessageResponse>
    {
        public UpdateBrandStatusRequest(Guid brandId, Status status)
        {
            BrandId = brandId;
            Status = status;
        }

        public Guid BrandId { get; set; }
        public Status Status { get; set; }
    }
}