using System.Linq;
using FluentValidation;
using TGS.Apis.AgentApi.Models;
using TGS.Core.Models.Enums;

namespace TGS.Apis.AgentApi.Validators;

public sealed class GetAccountChangesRequestValidator : AbstractValidator<GetAccountChangesRequest>
{
    public GetAccountChangesRequestValidator()
    {
        var allowType = new[]
        {
            Models.Enums.TransactionType.Deposit,
            Models.Enums.TransactionType.Withdrawal,
            Models.Enums.TransactionType.CancelWithdrawal,
            Models.Enums.TransactionType.CompanyAdjust,
            Models.Enums.TransactionType.AgentDividend
        };

        RuleFor(r => r.TransactionType)
            .Must(r => allowType.Contains(r.Value))
            .When(r => r.TransactionType != null)
            .WithMessage($"TransactionType should contain only {string.Join(",", allowType)}");
        ;
    }
}