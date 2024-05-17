using FluentValidation;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.AgentApi.Validators;

public sealed class
    ChangePayPasswordByValidateCodeRequestValidator : AbstractValidator<ChangePayPasswordByValidateCodeRequest>
{
    public ChangePayPasswordByValidateCodeRequestValidator() => RuleFor(x => x.NewPassword)
        .NotEmpty()
        .Length(6, 20)
        .WithName(x => x
            .WithPropertyExpression(o => o.NewPassword)
            .GetBindPropertyName(nameof(x.NewPassword)));
}