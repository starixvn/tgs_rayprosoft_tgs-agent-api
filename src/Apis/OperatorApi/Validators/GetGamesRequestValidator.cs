using System;
using System.Linq;
using FluentValidation;
using FluentValidation.Validators;
using TGS.Apis.OperatorApi.Models.Messages;
using TGS.Core.Infrastructures.WebApiServices;
using TGS.Core.Models.Enums;

namespace TGS.Apis.OperatorApi.Validators
{
    public sealed class GetGamesRequestValidator : BaseFluentValidator<GetGamesRequest>
    {
        public GetGamesRequestValidator()
        {
            RuleFor(x => x.GameProviderCode)
                .NotEmpty()
                .WithName(x => GetBindPropertyName(x, nameof(x.GameProviderCode)));

            RuleFor(x => x.LanguageCode)
                .NotEmpty()
                .WithName(x => GetBindPropertyName(x, nameof(x.LanguageCode)));

            RuleFor(x => x.CurrencyCode)
                .NotEmpty()
                .WithName(x => GetBindPropertyName(x, nameof(x.CurrencyCode)));

            RuleFor(x => x.PlatformType)
                .IsInEnum()
                .WithMessage(x => $"{GetBindPropertyName(x, nameof(x.PlatformType))} does not exist");

            RuleFor(x => x.Types)
                .Must(ValidateGameTypes)
                .When(x => x.Types != null && x.Types.Length > 0)
                .WithMessage(x => $"one of the {GetBindPropertyName(x, nameof(x.Types))} does not exist");
        }

        private static bool ValidateGameTypes(GameType[] gameTypes) => 
            gameTypes.All(type => Enum.IsDefined(typeof(GameType), type));
    }
}
