using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TGS.Core.Models;
using TGS.Core.Models.Enums;
using TGS.Core.Models.Messages;
using TGS.Services.Promotion.Models.Promotion;

namespace TGS.Services.Promotion.Handlers
{
    public class CreatePromotionParticipantHandler : Handler<CreatePromotionParticipantRequest, MessageResponse>
    {
        private readonly ILogger<CreatePromotionParticipantHandler> _logger;
        private readonly IPromotionParticipantRepository _promotionParticipantRepository;
        private readonly IPromotionRepository _promotionRepository;
        private readonly IPromotionTypeFactory _promotionType;

        public CreatePromotionParticipantHandler(
            IPromotionRepository promotionRepository,
            IPromotionParticipantRepository promotionParticipantRepository,
            IPromotionTypeFactory promotionType,
            IHttpContextAccessor accessor,
            ILogger<CreatePromotionParticipantHandler> logger) : base(accessor)
        {
            _promotionRepository = promotionRepository ?? throw new ArgumentNullException(nameof(promotionRepository));
            _promotionParticipantRepository = promotionParticipantRepository ??
                throw new ArgumentNullException(nameof(promotionParticipantRepository));
            _promotionType = promotionType ??
                throw new ArgumentNullException(nameof(promotionType));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task<MessageResponse> Handle(CreatePromotionParticipantRequest request,
            CancellationToken cancellationToken)
        {
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

            if (await _promotionType.IsDepositTypeAsync(promotion.PromotionType))
            {
                _logger.LogWarning(
                    $"Promotion({request.PromotionId}) Type({Consts.PromotionGroupType.Deposit}) invalid.");

                return CreateInvalidResponse<MessageResponse>($"{Consts.PromotionGroupType.Deposit} invalid");
            }

            await _promotionParticipantRepository.CreatePromotionParticipantAsync(request.PromotionId,
                request.PlayerCode);

            return CreateValidResponse<MessageResponse>();
        }
    }
}