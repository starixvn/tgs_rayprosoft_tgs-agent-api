using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.OperatorApi.Validators
{
    public sealed class ChangePasswordRequestValidator : BaseFluentValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.OldPassword)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.OldPassword)));

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.NewPassword)));

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.ConfirmPassword)));

            RuleFor(x => x.NewPassword)
                .Equal(x => x.ConfirmPassword)
                .WithMessage(x =>
                    $"{GetJsonPropertyName(x, nameof(x.NewPassword))} is not the same as {GetJsonPropertyName(x, nameof(x.ConfirmPassword))}");
        }
    }
}