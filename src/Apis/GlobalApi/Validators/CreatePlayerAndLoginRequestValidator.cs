using FluentValidation;
using TGS.Apis.GlobalApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.GlobalApi.Validators
{
    public class CreatePlayerAndLoginRequestValidator : BaseFluentValidator<CreatePlayerAndLoginRequest>
    {
        public CreatePlayerAndLoginRequestValidator()
        {
            RuleFor(x => x.BrandCode)
                .NotNull()
                .WithName(x => GetJsonPropertyName(x, nameof(x.BrandCode)));

            RuleFor(x => x.CurrencyCode)
                .NotNull()
                .WithName(x => GetJsonPropertyName(x, nameof(x.CurrencyCode)));

            RuleFor(x => x.Username)
                .NotNull()
                .WithName(x => GetJsonPropertyName(x, nameof(x.Username)));

            RuleFor(x => x.InitialWalletAmount)
                .GreaterThan(0)
                .WithName(x => GetJsonPropertyName(x, nameof(x.InitialWalletAmount)));
        }
    }
}