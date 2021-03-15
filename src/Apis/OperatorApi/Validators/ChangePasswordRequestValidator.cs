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
        }
    }
}