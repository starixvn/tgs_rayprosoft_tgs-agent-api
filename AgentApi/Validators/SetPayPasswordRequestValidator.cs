using FluentValidation;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.AgentApi.Validators;

public sealed class SetPayPasswordRequestValidator : AbstractValidator<SetPayPasswordRequest>
{
    public SetPayPasswordRequestValidator() => RuleFor(x => x.Password)
        .NotEmpty()
        .Length(6, 20)
        .WithName(x => x
            .WithPropertyExpression(o => o.Password)
            .GetBindPropertyName(nameof(x.Password)));
}