using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;

namespace TGS.Apis.OperatorApi.Validators
{
    public sealed class CreatePlayerRequestValidator : BaseUserRequestValidator<CreatePlayerRequest>
    {
        public CreatePlayerRequestValidator()
        {
            RuleFor(x => x.CurrencyCode)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.CurrencyCode)));

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.Password)));

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.ConfirmPassword)));

            RuleFor(x => x.Password)
                .Equal(x => x.ConfirmPassword)
                .WithMessage(x => $"{GetJsonPropertyName(x, nameof(x.Password))} does not equals to {GetJsonPropertyName(x, nameof(x.ConfirmPassword))}");
        }
    }
}