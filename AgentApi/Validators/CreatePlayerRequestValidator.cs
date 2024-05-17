using FluentValidation;
using FluentValidation.Validators;
using TGS.Apis.AgentApi.Models;

namespace TGS.Apis.AgentApi.Validators;

public sealed class CreatePlayerRequestValidator : AbstractValidator<CreatePlayerRequest>
{
    public CreatePlayerRequestValidator()
    {
        _ = RuleFor(x => x.Username)
            .NotEmpty()
            .Length(6, 32)
            .SetValidator(new RegularExpressionValidator<CreatePlayerRequest>("^[0-9a-zA-Z]+$"));

        _ = RuleFor(x => x.Password)
            .NotEmpty()
            .Length(6, 20);
    }
}