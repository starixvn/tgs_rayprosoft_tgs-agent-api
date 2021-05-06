using FluentValidation;
using FluentValidation.Validators;
using TGS.Apis.OperatorApi.Models.Messages;

namespace TGS.Apis.OperatorApi.Validators
{
    public sealed class CreatePlayerRequestValidator : BaseUserRequestValidator<CreatePlayerRequest>
    {
        public CreatePlayerRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .WithName(x=> GetJsonPropertyName(x,nameof(x.Username)))
                .Length(6,32)                
                .SetValidator(new RegularExpressionValidator("^[0-9a-zA-Z]+$"));
                
            RuleFor(x => x.CurrencyCode)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.CurrencyCode)));

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.Password)))
                .Length(6,20);

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.ConfirmPassword)));

            RuleFor(x => x.Password)
                .Equal(x => x.ConfirmPassword)
                .WithMessage(x => $"{GetJsonPropertyName(x, nameof(x.Password))} does not equals to {GetJsonPropertyName(x, nameof(x.ConfirmPassword))}");
        }
    }
}