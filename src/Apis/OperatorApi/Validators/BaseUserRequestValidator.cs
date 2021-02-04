using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.OperatorApi.Validators
{
    public abstract class BaseUserRequestValidator<T> : BaseFluentValidator<T> where T: BaseUserRequest
    {
        protected BaseUserRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(6, 32)
                .WithName(x => GetJsonPropertyName(x, nameof(x.Username)));
        }
    }
}
