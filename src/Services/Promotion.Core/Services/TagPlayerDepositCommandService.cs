using System;
using System.Threading;
using System.Threading.Tasks;
using TGS.ServiceUtility;

namespace TGS.Services.Promotion.Services
{
    internal class TagPlayerDepositCommandService : ICommandService<TagPlayerDepositCommand>
    {
        private readonly IPromotionChecker[] _checkers;

        public TagPlayerDepositCommandService(IPromotionCheckerFactory promotionCheckerFactory)
        {
            if (promotionCheckerFactory is null)
            {
                throw new ArgumentNullException(nameof(promotionCheckerFactory));
            }

            var firstDepositChecker = promotionCheckerFactory.GetPromotionChecker("FirstDeposit");
            var dailyFirstDepositChecker = promotionCheckerFactory.GetPromotionChecker("DailyFirstDeposit");

            _checkers = new[] { firstDepositChecker, dailyFirstDepositChecker };
        }

        public async ValueTask ExecuteAsync(TagPlayerDepositCommand command, CancellationToken cancellationToken = default)
        {
            foreach (var checker in _checkers)
                await checker
                    .TagPromotionTypeAsync(command.PlayerCode)
                    .ConfigureAwait(false);
        }
    }
}
