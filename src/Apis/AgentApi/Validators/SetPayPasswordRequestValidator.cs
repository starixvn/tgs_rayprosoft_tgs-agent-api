using FluentValidation;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.AgentApi.Validators
{
    public sealed class SetPayPasswordRequestValidator : BaseFluentValidator<SetPayPasswordRequest>
    {
        public SetPayPasswordRequestValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty()
                .Length(6, 20)
                .WithName(x => GetBindPropertyName(x, nameof(x.Password)));
        }
    }
}
