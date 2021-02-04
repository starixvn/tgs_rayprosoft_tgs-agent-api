using FluentValidation;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;

namespace TGS.Apis.OperatorApi.Validators
{
    public sealed class GetGameUrlRequestValidator : BaseFluentValidator<GetGameUrlRequest>
    {
        public GetGameUrlRequestValidator()
        {
            RuleFor(x => x.GameProviderCode)
                .NotEmpty()
                .WithName(x => GetBindPropertyName(x, nameof(x.GameProviderCode)));

            RuleFor(x => x.GameId)
                .NotEmpty()
                .WithName(x => GetBindPropertyName(x, nameof(x.GameId)));

            RuleFor(x => x.LanguageCode)
                .NotEmpty()
                .WithName(x => GetBindPropertyName(x, nameof(x.LanguageCode)));
        }
    }
}