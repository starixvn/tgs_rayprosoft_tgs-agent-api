using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.OperatorApi.Validators
{
    public sealed class CheckUsernameRequestValidator : BaseFluentValidator<CheckUsernameRequest>
    {
        public CheckUsernameRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(6, 32)
                .WithName(x => GetBindPropertyName(x, nameof(x.Username)));
        }
    }
}