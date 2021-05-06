using System;
using MediatR;
using TGS.Core.Models.Messages;

namespace TGS.Services.Promotion.Core.Models.Promotion
{
    public record CreatePromotionParticipantRequest(Guid PromotionId, string PlayerCode) : IRequest<MessageResponse>;
}