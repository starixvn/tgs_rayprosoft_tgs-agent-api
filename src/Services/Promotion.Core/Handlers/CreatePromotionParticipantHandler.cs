using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Promotion.Models.Promotion;

namespace TGS.Services.Promotion.Handlers
{
    public class CreatePromotionParticipantHandler : Handler<CreatePromotionParticipantRequest, MessageResponse>
    {
        private readonly IPromotionRepository _promotionRepository;
        private readonly IPromotionParticipantRepository _promotionParticipantService;
        private readonly ILogger<CreatePromotionParticipantHandler> _logger;

        public CreatePromotionParticipantHandler(
            IPromotionRepository promotionRepository,
            IPromotionParticipantRepository promotionParticipantService,
            IHttpContextAccessor accessor,
            ILogger<CreatePromotionParticipantHandler> logger) : base(accessor)
        {
            _promotionRepository = promotionRepository ?? throw new System.ArgumentNullException(nameof(promotionRepository));
            _promotionParticipantService = promotionParticipantService ?? throw new System.ArgumentNullException(nameof(promotionParticipantService));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public override async Task<MessageResponse> Handle(CreatePromotionParticipantRequest request,
            CancellationToken cancellationToken)
        {
            if (await _promotionParticipantService.ExistsAsync(x =>
                x.PromotionId == request.PromotionId && x.PlayerCode == request.PlayerCode))
            {
                _logger.LogWarning(Properties.Promotion.PromotionJoinedAlready);

                return CreateInvalidResponse<MessageResponse>(Properties.Promotion.PromotionJoinedAlready);
            }

            var promotion = await _promotionRepository
                .GetPromotionByIdAsync(request.PromotionId)
                .ConfigureAwait(false);

            if (promotion == null)
            {
                _logger.LogWarning($"Promotion({request.PromotionId}) is null");

                return CreateInvalidResponse<MessageResponse>("Can't find promotion");
            }

            var currentDate = DateTimeOffset.Now;

            if (promotion.Status != SwitchStatus.Enabled
                && promotion.From <= currentDate
                && promotion.To > currentDate)
            {
                _logger.LogWarning($"Promotion({request.PromotionId}) invalid.");

                return CreateInvalidResponse<MessageResponse>("Promotion invalid");
            }

            await _promotionParticipantService.CreatePromotionParticipantAsync(request.PromotionId, request.PlayerCode);

            return CreateValidResponse<MessageResponse>();
        }
    }
}
