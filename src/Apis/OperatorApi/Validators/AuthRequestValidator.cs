using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.OperatorApi.Validators
{
    public sealed class AuthRequestValidator : BaseFluentValidator<AuthRequest>
    {
        public AuthRequestValidator()
        {
            RuleFor(x => x.ApiKey)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.ApiKey)));

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.Currency)));
        }
    }
}
