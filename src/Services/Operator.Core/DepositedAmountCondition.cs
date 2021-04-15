using System.Threading.Tasks;

namespace TGS.Services.Operator
{
    internal class DepositedAmountCondition : IBrandPaymentCondition
    {
        public ValueRange<decimal> DepositedAmountLimit { get; init; }

        public ValueTask<bool> IsValidAsync(PlayerInfo player)
            => ValueTask.FromResult(player.DepositedAmount > DepositedAmountLimit.From
                && player.DepositedAmount <= DepositedAmountLimit.To);
    }
}
