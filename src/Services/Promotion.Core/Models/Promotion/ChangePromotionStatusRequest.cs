using System;
using MediatR;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Core.Models.Promotion
{
    public record ChangePromotionStatusRequest(Guid Id, SwitchStatus Status) : IRequest<MessageResponse>;
}