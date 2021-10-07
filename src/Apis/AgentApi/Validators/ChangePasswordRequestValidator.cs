using FluentValidation;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.AgentApi.Validators
{
    public sealed class ChangePasswordRequestValidator : BaseFluentValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        { 
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .Length(6, 20)
                .WithName(x => GetJsonPropertyName(x, nameof(x.NewPassword)));
        }
    }
}
