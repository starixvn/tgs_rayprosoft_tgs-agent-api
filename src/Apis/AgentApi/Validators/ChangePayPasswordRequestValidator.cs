using FluentValidation;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.AgentApi.Validators
{
    public sealed class ChangePayPasswordRequestValidator : BaseFluentValidator<ChangePayPasswordRequest>
    {
        public ChangePayPasswordRequestValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .Length(6, 20)
                .WithName(x => GetBindPropertyName(x, nameof(x.NewPassword)));
        }
    }
}
