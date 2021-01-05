using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.OperatorApi.Validators
{
    public sealed class GetGameProvidersRequestValidator : BaseFluentValidator<GetGameProvidersRequest>
    {
        public GetGameProvidersRequestValidator()
        {
            RuleFor(x => x.CurrencyCode)
                .NotEmpty()
                .WithName(x => GetBindPropertyName(x, nameof(x.CurrencyCode)));

            RuleFor(x => x.LanguageCode)
                .NotEmpty()
                .WithName(x => GetBindPropertyName(x, nameof(x.LanguageCode)));

            RuleFor(x => x.PlatformType)
                .IsInEnum()
                .WithMessage(x => $"{GetBindPropertyName(x, nameof(x.PlatformType))} does not exist");
        }
    }
}