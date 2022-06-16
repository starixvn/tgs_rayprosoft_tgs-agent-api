using FluentValidation;
using TGS.Apis.AgentApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.AgentApi.Validators;

public class ChangePayPasswordRequestValidator : BaseFluentValidator<ChangePayPasswordRequest>
{
    public ChangePayPasswordRequestValidator()
    {
        RuleFor(request => request.NewPassword)
            .NotEmpty()
            .Length(6, 20)
            .WithName(x => GetBindPropertyName(x, nameof(x.NewPassword)));
    }
}
