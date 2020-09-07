using TGS.Core.Models.Exceptions;

namespace TGS.Services.Operations.Validators
{
    public interface IAmountValidator
    {
        void ThrowIfAmountIsNotPositive(decimal amount);
        void ThrowIfAmountIsNegative(decimal amount);
        void IgnoreAmount(decimal amount);
    }

    public sealed class AmountValidator : IAmountValidator
    {
        void IAmountValidator.IgnoreAmount(decimal amount)
        {
            
        }

        void IAmountValidator.ThrowIfAmountIsNegative(decimal amount)
        {            
            if (amount < 0)
            {
                throw new InvalidAmountException($"The amount {amount} must be zero or positive");
            }
        }

        void IAmountValidator.ThrowIfAmountIsNotPositive(decimal amount)
        {
            if (amount <= 0)
            {
                throw new InvalidAmountException($"The amount {amount} must be positive");
            }
        }
    }
}