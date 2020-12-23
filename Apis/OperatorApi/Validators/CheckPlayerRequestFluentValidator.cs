using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.OperatorApi.Validators
{
    public sealed class CheckPlayerRequestFluentValidator : BaseFluentValidator<CheckPlayerRequest>
    {
        public CheckPlayerRequestFluentValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.Username)));
        }
    }
}