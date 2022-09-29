using FluentValidation;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Core.Infrastructures.Extensions;

namespace TGS.Apis.AgentApi.Validators;

public class ChangePayPasswordRequestValidator : AbstractValidator<ChangePayPasswordRequest>
{
    public ChangePayPasswordRequestValidator()
    {
        RuleFor(request => request.NewPassword)
            .NotEmpty()
            .Length(6, 20)
            .WithName(x => x
                .WithPropertyExpression(o => o.NewPassword)
                .GetBindPropertyName(nameof(x.NewPassword)));
    }
}
