using FluentValidation;

namespace TGS.Apis.BackendOffice.Extensions
{
    public static class RuleBuilderExtensions
    {
        public static IRuleBuilder<T, string> Password<T>(this IRuleBuilder<T, string> ruleBuilder,
            int minimumLength = 8)
        {
            var options = ruleBuilder
                .NotEmpty().WithMessage(Resources.User.PasswordRequired)
                .MinimumLength(minimumLength).WithMessage(Resources.User.PasswordLengthAtLeast)
                .Matches("[A-Z]").WithMessage(Resources.User.PasswordNeedUpperCaseLetter)
                .Matches("[a-z]").WithMessage(Resources.User.PasswordNeedLowerCaseLetter)
                .Matches("[0-9]").WithMessage(Resources.User.PasswordNeedNumber)
                .Matches("[^a-zA-Z0-9]").WithMessage(Resources.User.PasswordNeedSpecialCharacter);
            return options;
        }
    }
}
