using FluentValidation;
using TGS.Apis.BackendOffice.Resources;

namespace TGS.Apis.BackendOffice.Extensions
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder,
            int minimumLength = 8)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage(User.PasswordRequired)
                .MinimumLength(minimumLength).WithMessage(User.PasswordLengthAtLeast)
                .Matches("[0-9]").WithMessage(User.PasswordNeedNumber)
                .Matches("[^a-zA-Z0-9]").WithMessage(User.PasswordNeedSpecialCharacter);
            return options;
        }
    }
}