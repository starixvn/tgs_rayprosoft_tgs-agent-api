using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.OperatorApi.Validators
{
    public sealed class GetGamesRequestValidator : BaseFluentValidator<GetGamesRequest>
    {
        public GetGamesRequestValidator()
        {
            RuleFor(x => x.GameProviderCode)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.GameProviderCode)));

            RuleFor(x => x.LanguageCode)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.LanguageCode)));

            RuleFor(x => x.CurrencyCode)
                .NotEmpty()
                .WithName(x => GetJsonPropertyName(x, nameof(x.CurrencyCode)));

            RuleFor(x => x.PlatformType)
                .IsInEnum()
                .WithMessage(x => $"{GetJsonPropertyName(x, nameof(x.PlatformType))} does not exist");
        }
    }
}
