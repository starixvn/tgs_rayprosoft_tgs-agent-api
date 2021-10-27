using System.Linq;
using FluentValidation;
using TGS.Apis.AgentApi.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.AgentApi.Validators
{
    public sealed class GetAccountChangesRequestValidator : AbstractValidator<GetAccountChangesRequest>
    {
        public GetAccountChangesRequestValidator()
        {
            var allowType = new TransactionType[]
            {
                TransactionType.Deposit,
                TransactionType.Withdrawal,
                TransactionType.CancelWithdrawal,
                TransactionType.Adjust,
                TransactionType.Dividend
            };

            RuleFor(r => r.TransactionType)
                .Must(r => allowType.Contains(r.Value))
                .When(r => r.TransactionType != null)
                .WithMessage($"TransactionType should contain only {string.Join(",", allowType)}"); ;
        }
    }
}
