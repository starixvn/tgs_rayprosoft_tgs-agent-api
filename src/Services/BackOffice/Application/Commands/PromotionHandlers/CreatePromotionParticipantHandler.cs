using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using TGS.Core.Models.Messages;
using TGS.Services.Promotion.Core;
using TGS.Services.Promotion.Core.Models.Promotion;

namespace TGS.Services.BackOffice.Application.Commands.PromotionHandlers
{
    public class CreatePromotionParticipantHandler : Handler<CreatePromotionParticipantRequest, MessageResponse>
    {
        private readonly IPromotionParticipantRepository _promotionParticipantService;

        public CreatePromotionParticipantHandler(IPromotionParticipantRepository promotionParticipantService,
            IHttpContextAccessor accessor) : base(accessor)
        {
            _promotionParticipantService = promotionParticipantService;
        }

        public override async Task<MessageResponse> Handle(CreatePromotionParticipantRequest request,
            CancellationToken cancellationToken)
        {
            if (await _promotionParticipantService.ExistsAsync(x =>
                x.PromotionId == request.PromotionId && x.PlayerCode == request.PlayerCode))
            {
                return CreateInvalidResponse<MessageResponse>(Resources.Promotion.PromotionJoinedAlready);
            }

            await _promotionParticipantService.CreatePromotionParticipantAsync(request.PromotionId, request.PlayerCode);

            return CreateValidResponse<MessageResponse>();
        }
    }
}