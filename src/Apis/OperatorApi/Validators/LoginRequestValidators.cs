using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;

namespace TGS.Apis.OperatorApi.Validators
{
    public sealed class LoginRequestValidators : BaseUserRequestValidator<LoginRequest>
    {
        public LoginRequestValidators()
        {
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, x.Password));

            RuleFor(x => x.PlatformType)
                .IsInEnum()
                .WithMessage(x => $"{GetJsonPropertyName(x, nameof(x.PlatformType))} does not exist");
        }
    }
}