using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Promotion.Services
{
    internal class RevertPromotionAmountCommandService : ICommandService<RevertPromotionAmountCommand>
    {
        private readonly IPromotionRepository _promotionRepository;

        public RevertPromotionAmountCommandService(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository ?? throw new ArgumentNullException(nameof(promotionRepository));
        }

        public async ValueTask ExecuteAsync(
            RevertPromotionAmountCommand command,
            CancellationToken cancellationToken = default)
        {
            var promotion = await _promotionRepository.GetPromotionByIdAsync(command.PromotionId);

            var amount = promotion.Percentage * command.Amount / 100M;
            amount = amount > promotion.BonusAmountLimit ? promotion.BonusAmountLimit : amount;

            _ = await _promotionRepository
                .RevertPromotionAmountAsync(command.PromotionId, amount)
                .ConfigureAwait(false);
        }
    }
}